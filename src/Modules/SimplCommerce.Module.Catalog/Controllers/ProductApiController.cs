using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using SimplCommerce.Infrastructure;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Web.SmartTable;
using SimplCommerce.Module.Catalog.Models;
using SimplCommerce.Module.Catalog.Services;
using SimplCommerce.Module.Catalog.ViewModels;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Core.Services;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Extensions.Constants;
using AutoMapper;
using SimplCommerce.Module.Catalog.Services.Dtos;

namespace SimplCommerce.Module.Catalog.Controllers
{
    [Authorize(Policy.CanAccessDashboard)]
    [Route("api/products")]
    [ApiController]
    public class ProductApiController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMediaService _mediaService;
        private readonly IRepository<ProductAttributeValue> _productAttributeValueRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IRepository<ProductLink> _productLinkRepository;
        private readonly IRepository<ProductOptionValue> _productOptionValueRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IAuthorizationService _authorizationService;

        public ProductApiController(
            IMapper mapper,
            IRepository<Product> productRepository,
            IMediaService mediaService,
            IProductService productService,
            IRepository<ProductLink> productLinkRepository,
            IRepository<ProductCategory> productCategoryRepository,
            IRepository<ProductOptionValue> productOptionValueRepository,
            IRepository<ProductAttributeValue> productAttributeValueRepository,
            IWorkContext workContext,
            IAuthorizationService authorizationService)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _mediaService = mediaService;
            _productService = productService;
            _productLinkRepository = productLinkRepository;
            _productCategoryRepository = productCategoryRepository;
            _productOptionValueRepository = productOptionValueRepository;
            _productAttributeValueRepository = productAttributeValueRepository;
            _workContext = workContext;
            _authorizationService = authorizationService;
        }

        [HttpGet("search")]
        public async Task<ActionResult<IList<ProductDto>>> Search(string query)
        {
            const int maxItems = 100;
            var products = await _productService.SearchAsync(query, maxItems);
            return products.ToList();
        }

        [HttpGet("setting")]
        public async Task<IActionResult> GetSetting()
        {
            var result = await _productService.GetProductSettingAsync();
            return Ok(result);
        }

        [HttpPost("addStock/{barcode}")]
        public async Task<ActionResult<ObjectResult>> AddStock(string barcode)
        {
            var (ok, error) = await _productService.AddStockAsync(barcode);

            return Ok(ok);
        }

        [HttpGet("{id}")]
        [Authorize(Policy.CanEditProduct)]
        public async Task<IActionResult> Get(long id)
        {
            var product = _productRepository.Query()
                .Include(x => x.ThumbnailImage)
                .Include(x => x.Medias).ThenInclude(m => m.Media)
                .Include(x => x.ProductLinks).ThenInclude(p => p.LinkedProduct)
                .Include(x => x.OptionValues).ThenInclude(o => o.Option)
                .Include(x => x.AttributeValues).ThenInclude(a => a.Attribute).ThenInclude(g => g.Group)
                .Include(x => x.Categories)
                .FirstOrDefault(x => x.Id == id);

            var currentUser = await _workContext.GetCurrentUser();
            if (!User.IsInRole(RoleName.Admin) && product.VendorId != currentUser.VendorId)
            {
                return new BadRequestObjectResult(new { error = "You don't have permission to manage this product" });
            }

            var productVm = _mapper.Map<Product, ProductVm>(product,
                opt => opt.AfterMap((src, dest) =>
                {
                    dest.ThumbnailImageUrl = _mediaService.GetThumbnailUrl(product.ThumbnailImage);
                }));
            productVm.ProductImages = _mapper.Map<IList<ProductMediaVm>>(
                product.GetMediasWithUrl(MediaType.Image, media => _mediaService.GetThumbnailUrl(media)));
            productVm.ProductDocuments = _mapper.Map<IList<ProductMediaVm>>(
                product.GetMediasWithUrl(MediaType.File, media => _mediaService.GetMediaUrl(media)));
            productVm.Options = _mapper.Map<IList<ProductOptionVm>>(product.OptionValues.OrderBy(i => i.SortIndex));
            productVm.Variations = _mapper.Map<IList<ProductVariationVm>>(product.GetLinkedProducts(ProductLinkType.Super));
            productVm.RelatedProducts = _mapper.Map<IList<ProductLinkVm>>(product.GetLinkedProducts(ProductLinkType.Related));
            productVm.CrossSellProducts = _mapper.Map<IList<ProductLinkVm>>(product.GetLinkedProducts(ProductLinkType.CrossSell));
            productVm.Attributes = _mapper.Map<IList<ProductAttributeVm>>(product.AttributeValues);

            return Json(productVm);
        }

        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] SmartTableParam param)
        {
            var query = _productRepository.Query().Where(x => !x.IsDeleted);
            var currentUser = await _workContext.GetCurrentUser();
            var canManageOrder = (await _authorizationService.AuthorizeAsync(User, Policy.CanManageOrder)).Succeeded;
            query = query.WhereIf(!canManageOrder, x => x.VendorId == currentUser.VendorId);

            if (param.Search.PredicateObject != null)
            {
                dynamic search = param.Search.PredicateObject;
                var name = (string)search.Name;
                var sku = (string)search.Sku;
                var hasOptions = (bool?)search.HasOptions;
                var isVisibleIndividually = (bool?)search.IsVisibleIndividually;
                var isPublished = (bool?)search.IsPublished;
                var before = (DateTimeOffset?)search.CreatedOn?.before;
                var after = (DateTimeOffset?)search.CreatedOn?.after;

                query = query
                    .WhereIf(!name.IsNullOrEmpty(), x => x.Name.Contains(name))
                    .WhereIf(!sku.IsNullOrEmpty(), x => x.Sku.Contains(sku))
                    .WhereIf(hasOptions.HasValue, x => x.HasOptions == hasOptions)
                    .WhereIf(isVisibleIndividually.HasValue, x => x.IsVisibleIndividually == isVisibleIndividually)
                    .WhereIf(isPublished.HasValue, x => x.IsPublished == isPublished)
                    .WhereIf(before.HasValue, x => x.CreatedOn <= before)
                    .WhereIf(after.HasValue, x => x.CreatedOn >= after)
                    ;
            }

            var gridData = query.ToSmartTableResult(param, item => _mapper.Map<ProductListItem>(item));

            return Json(gridData);
        }

        [HttpPost]
        [Authorize(Policy.CanEditProduct)]
        public async Task<IActionResult> Post([FromForm] ProductForm model)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var currentUser = await _workContext.GetCurrentUser();

            var product = _mapper.Map<ProductVm, Product>(model.Product,
                opt => opt.AfterMap((src, dest) =>
                {
                    dest.CreatedBy = currentUser;
                    dest.VendorId = User.IsInRole(RoleName.Admin) ? null : currentUser.VendorId;
                    dest.IsVisibleIndividually = true; // put this config here because it's specific for this case
                }));

            var optionIndex = 0;
            foreach (var option in model.Product.Options)
            {
                product.AddOptionValue(new ProductOptionValue
                {
                    OptionId = option.Id,
                    DisplayType = option.DisplayType,
                    Value = JsonConvert.SerializeObject(option.Values),
                    SortIndex = optionIndex
                });

                optionIndex++;
            }

            foreach (var attribute in model.Product.Attributes)
            {
                var attributeValue = new ProductAttributeValue
                {
                    AttributeId = attribute.Id,
                    Value = attribute.Value
                };
                product.AddAttributeValue(attributeValue);
            }

            foreach (var categoryId in model.Product.CategoryIds)
            {
                var productCategory = new ProductCategory
                {
                    CategoryId = categoryId
                };
                product.AddCategory(productCategory);
            }

            await SaveProductMedias(model, product);

            MapProductVariationVmToProduct(model, product);
            MapProductLinkVmToProduct(model, product);

            _productService.Create(product);

            return Accepted(new { product.Id });
        }

        [HttpPut("{id}")]
        [Authorize(Policy.CanEditProduct)]
        public async Task<IActionResult> Put(long id, [FromForm] ProductForm model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = _productRepository.Query()
                .Include(x => x.ThumbnailImage)
                .Include(x => x.Medias).ThenInclude(m => m.Media)
                .Include(x => x.ProductLinks).ThenInclude(x => x.LinkedProduct)
                .Include(x => x.OptionValues).ThenInclude(o => o.Option)
                .Include(x => x.AttributeValues).ThenInclude(a => a.Attribute).ThenInclude(g => g.Group)
                .Include(x => x.Categories)
                .FirstOrDefault(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var currentUser = await _workContext.GetCurrentUser();
            if (!User.IsInRole(RoleName.Admin) && product.VendorId != currentUser.VendorId)
            {
                return new BadRequestObjectResult(new { error = "You don't have permission to manage this product" });
            }

            product = _mapper.Map(model.Product, product,
                opt => opt.AfterMap((src, dest) => dest.UpdatedBy = currentUser));

            await SaveProductMedias(model, product);

            foreach (var productMediaId in model.Product.DeletedMediaIds)
            {
                var productMedia = product.Medias.First(x => x.Id == productMediaId);
                await _mediaService.DeleteMediaAsync(productMedia.Media);
                product.RemoveMedia(productMedia);
            }

            AddOrDeleteProductOption(model.Product, product);
            AddOrDeleteProductAttribute(model, product);
            AddOrDeleteCategories(model, product);
            AddOrDeleteProductVariation(model.Product, product);
            AddOrDeleteProductLinks(model, product);

            _productService.Update(product);

            return Accepted(new { product.Id });
        }

        [HttpPost("change-status/{id}")]
        public async Task<IActionResult> ChangeStatus(long id)
        {
            var product = _productRepository.Query().FirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            var currentUser = await _workContext.GetCurrentUser();
            if (!User.IsInRole("admin") && product.VendorId != currentUser.VendorId)
            {
                return BadRequest(new { error = "You don't have permission to manage this product!" });
            }

            product.IsPublished = !product.IsPublished;
            await _productRepository.SaveChangesAsync();

            return Accepted(new { product.Id });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy.CanEditProduct)]
        public async Task<IActionResult> Delete(long id)
        {
            var product = _productRepository.Query().FirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            var currentUser = await _workContext.GetCurrentUser();
            if (!User.IsInRole(RoleName.Admin) && product.VendorId != currentUser.VendorId)
            {
                return new BadRequestObjectResult(new { error = "You don't have permission to manage this product" });
            }

            await _productService.DeleteAsync(product);

            return NoContent();
        }

        private static void MapProductVariationVmToProduct(ProductForm model, Product product)
        {
            foreach (var variationVm in model.Product.Variations)
            {
                var productLink = new ProductLink
                {
                    LinkType = ProductLinkType.Super,
                    Product = product,
                    LinkedProduct = product.Clone()
                };

                productLink.LinkedProduct.Name = variationVm.Name;
                productLink.LinkedProduct.SeoTitle = variationVm.Name.ToUrlFriendly();
                productLink.LinkedProduct.Price = variationVm.Price;
                productLink.LinkedProduct.OldPrice = variationVm.OldPrice;
                productLink.LinkedProduct.NormalizedName = variationVm.NormalizedName;
                productLink.LinkedProduct.HasOptions = false;
                productLink.LinkedProduct.IsVisibleIndividually = false;

                foreach (var combinationVm in variationVm.OptionCombinations)
                {
                    productLink.LinkedProduct.AddOptionCombination(new ProductOptionCombination
                    {
                        OptionId = combinationVm.OptionId,
                        Value = combinationVm.Value,
                        SortIndex = combinationVm.SortIndex
                    });
                }

                productLink.LinkedProduct.ThumbnailImage = product.ThumbnailImage;

                product.AddProductLinks(productLink);
            }
        }

        private static void MapProductLinkVmToProduct(ProductForm model, Product product)
        {
            foreach (var relatedProductVm in model.Product.RelatedProducts)
            {
                var productLink = new ProductLink
                {
                    LinkType = ProductLinkType.Related,
                    Product = product,
                    LinkedProductId = relatedProductVm.Id
                };

                product.AddProductLinks(productLink);
            }

            foreach (var crossSellProductVm in model.Product.CrossSellProducts)
            {
                var productLink = new ProductLink
                {
                    LinkType = ProductLinkType.CrossSell,
                    Product = product,
                    LinkedProductId = crossSellProductVm.Id
                };

                product.AddProductLinks(productLink);
            }
        }

        private void AddOrDeleteCategories(ProductForm model, Product product)
        {
            foreach (var categoryId in model.Product.CategoryIds)
            {
                if (product.Categories.Any(x => x.CategoryId == categoryId))
                {
                    continue;
                }

                var productCategory = new ProductCategory
                {
                    CategoryId = categoryId
                };
                product.AddCategory(productCategory);
            }

            var deletedProductCategories =
                product.Categories.Where(productCategory => !model.Product.CategoryIds.Contains(productCategory.CategoryId))
                    .ToList();

            foreach (var deletedProductCategory in deletedProductCategories)
            {
                deletedProductCategory.Product = null;
                product.Categories.Remove(deletedProductCategory);
                _productCategoryRepository.Remove(deletedProductCategory);
            }
        }

        private void AddOrDeleteProductOption(ProductVm productVm, Product product)
        {
            var optionIndex = 0;
            foreach (var optionVm in productVm.Options)
            {
                var optionValue = product.OptionValues.FirstOrDefault(x => x.OptionId == optionVm.Id);
                if (optionValue == null)
                {
                    product.AddOptionValue(new ProductOptionValue
                    {
                        OptionId = optionVm.Id,
                        DisplayType = optionVm.DisplayType,
                        Value = JsonConvert.SerializeObject(optionVm.Values),
                        SortIndex = optionIndex
                    });
                }
                else
                {
                    optionValue.Value = JsonConvert.SerializeObject(optionVm.Values);
                    optionValue.DisplayType = optionVm.DisplayType;
                    optionValue.SortIndex = optionIndex;
                }

                optionIndex++;
            }

            var deleteds =
                product.OptionValues.Where(x => productVm.Options.All(vm => vm.Id != x.OptionId)).ToList();
            foreach (var optionValue in deleteds)
            {
                product.OptionValues.Remove(optionValue);
                _productOptionValueRepository.Remove(optionValue);
            }

            product.HasOptions = product.OptionValues.Any();
        }

        private void AddOrDeleteProductVariation(ProductVm productVm, Product product)
        {
            foreach (var productVariationVm in productVm.Variations)
            {
                var productLink = product.ProductLinks.Where(x => x.LinkType == ProductLinkType.Super).FirstOrDefault(x => x.LinkedProduct.Name == productVariationVm.Name);
                if (productLink == null)
                {
                    productLink = new ProductLink
                    {
                        LinkType = ProductLinkType.Super,
                        Product = product,
                        LinkedProduct = product.Clone()
                    };

                    productLink.LinkedProduct.Name = productVariationVm.Name;
                    productLink.LinkedProduct.SeoTitle = StringHelper.ToUrlFriendly(productVariationVm.Name);
                    productLink.LinkedProduct.Price = productVariationVm.Price;
                    productLink.LinkedProduct.OldPrice = productVariationVm.OldPrice;
                    productLink.LinkedProduct.NormalizedName = productVariationVm.NormalizedName;
                    productLink.LinkedProduct.HasOptions = false;
                    productLink.LinkedProduct.IsVisibleIndividually = false;
                    productLink.LinkedProduct.ThumbnailImage = product.ThumbnailImage;

                    foreach (var combinationVm in productVariationVm.OptionCombinations)
                    {
                        productLink.LinkedProduct.AddOptionCombination(new ProductOptionCombination
                        {
                            OptionId = combinationVm.OptionId,
                            Value = combinationVm.Value,
                            SortIndex = combinationVm.SortIndex
                        });
                    }

                    product.AddProductLinks(productLink);
                }
                else
                {
                    productLink.LinkedProduct.Price = productVariationVm.Price;
                    productLink.LinkedProduct.OldPrice = productVariationVm.OldPrice;
                    productLink.LinkedProduct.IsDeleted = false;
                }
            }

            foreach (var productLink in product.ProductLinks.Where(x => x.LinkType == ProductLinkType.Super))
            {
                if (productVm.Variations.All(x => x.Name != productLink.LinkedProduct.Name))
                {
                    _productLinkRepository.Remove(productLink);
                    productLink.LinkedProduct.IsDeleted = true;
                }
            }
        }

        // Due to some issue with EF Core, we have to use _productLinkRepository in this case.
        private void AddOrDeleteProductLinks(ProductForm model, Product product)
        {
            foreach (var relatedProductVm in model.Product.RelatedProducts)
            {
                var productLink = product.ProductLinks.Where(x => x.LinkType == ProductLinkType.Related).FirstOrDefault(x => x.LinkedProductId == relatedProductVm.Id);
                if (productLink == null)
                {
                    productLink = new ProductLink
                    {
                        LinkType = ProductLinkType.Related,
                        Product = product,
                        LinkedProductId = relatedProductVm.Id,
                    };

                    _productLinkRepository.Add(productLink);
                }
            }

            foreach (var productLink in product.ProductLinks.Where(x => x.LinkType == ProductLinkType.Related))
            {
                if (model.Product.RelatedProducts.All(x => x.Id != productLink.LinkedProductId))
                {
                    _productLinkRepository.Remove(productLink);
                }
            }

            foreach (var crossSellProductVm in model.Product.CrossSellProducts)
            {
                var productLink = product.ProductLinks.Where(x => x.LinkType == ProductLinkType.CrossSell).FirstOrDefault(x => x.LinkedProductId == crossSellProductVm.Id);
                if (productLink == null)
                {
                    productLink = new ProductLink
                    {
                        LinkType = ProductLinkType.CrossSell,
                        Product = product,
                        LinkedProductId = crossSellProductVm.Id,
                    };

                    _productLinkRepository.Add(productLink);
                }
            }

            foreach (var productLink in product.ProductLinks.Where(x => x.LinkType == ProductLinkType.CrossSell))
            {
                if (model.Product.CrossSellProducts.All(x => x.Id != productLink.LinkedProductId))
                {
                    _productLinkRepository.Remove(productLink);
                }
            }
        }

        private void AddOrDeleteProductAttribute(ProductForm model, Product product)
        {
            foreach (var productAttributeVm in model.Product.Attributes)
            {
                var productAttrValue =
                    product.AttributeValues.FirstOrDefault(x => x.AttributeId == productAttributeVm.Id);
                if (productAttrValue == null)
                {
                    productAttrValue = new ProductAttributeValue
                    {
                        AttributeId = productAttributeVm.Id,
                        Value = productAttributeVm.Value
                    };
                    product.AddAttributeValue(productAttrValue);
                }
                else
                {
                    productAttrValue.Value = productAttributeVm.Value;
                }
            }

            var deletedAttrValues =
                product.AttributeValues.Where(attrValue => model.Product.Attributes.All(x => x.Id != attrValue.AttributeId))
                    .ToList();

            foreach (var deletedAttrValue in deletedAttrValues)
            {
                deletedAttrValue.Product = null;
                product.AttributeValues.Remove(deletedAttrValue);
                _productAttributeValueRepository.Remove(deletedAttrValue);
            }
        }

        private async Task SaveProductMedias(ProductForm model, Product product)
        {
            if (model.ThumbnailImage != null)
            {
                var fileName = await SaveFile(model.ThumbnailImage);
                if (product.ThumbnailImage != null)
                {
                    product.ThumbnailImage.FileName = fileName;
                }
                else
                {
                    product.ThumbnailImage = new Media { FileName = fileName };
                }
            }

            // Currently model binder cannot map the collection of file productImages[0], productImages[1]
            foreach (var file in Request.Form.Files)
            {
                if (file.ContentDisposition.Contains("productImages"))
                {
                    model.ProductImages.Add(file);
                }
                else if (file.ContentDisposition.Contains("productDocuments"))
                {
                    model.ProductDocuments.Add(file);
                }
            }

            foreach (var file in model.ProductImages)
            {
                var fileName = await SaveFile(file);
                var productMedia = new ProductMedia
                {
                    Product = product,
                    Media = new Media { FileName = fileName, MediaType = MediaType.Image }
                };
                product.AddMedia(productMedia);
            }

            foreach (var file in model.ProductDocuments)
            {
                var fileName = await SaveFile(file);
                var productMedia = new ProductMedia
                {
                    Product = product,
                    Media = new Media { FileName = fileName, MediaType = MediaType.File, Caption = file.FileName }
                };
                product.AddMedia(productMedia);
            }
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Value.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _mediaService.SaveMediaAsync(file.OpenReadStream(), fileName, file.ContentType);
            return fileName;
        }
    }
}
