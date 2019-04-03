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
using System.Text;
using CsvHelper;

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
        private readonly ISearchProductService _searchProductService;

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
            IAuthorizationService authorizationService,
            ISearchProductService searchProductService)
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
            _searchProductService = searchProductService;
        }

        [HttpGet("search")]
        public async Task<ActionResult<IList<ProductDto>>> Search(string query, bool? hasOptions)
        {
            const int maxItems = 100;
            var products = await _productService.SearchAsync(query, hasOptions, maxItems);
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

            return error.HasValue() ? (ObjectResult)BadRequest(error) : Ok(ok);
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
            var search = param.Search.PredicateObject?.ToObject<SearchProductParametersVm>() ?? new SearchProductParametersVm();

            var currentUser = await _workContext.GetCurrentUser();
            var canManageOrder = (await _authorizationService.AuthorizeAsync(User, Policy.CanManageOrder)).Succeeded;
            search.CanManageOrder = canManageOrder;
            search.VendorId = currentUser.VendorId;

            var query = _searchProductService.BuildQuery(search);

            var gridData = query.ToSmartTableResult(param, item => _mapper.Map<ProductListItem>(item));

            return Json(gridData);
        }

        [HttpPost("export")]
        public async Task<IActionResult> Export([FromBody] SmartTableParam param)
        {
            var search = param.Search.PredicateObject?.ToObject<SearchProductParametersVm>() ?? new SearchProductParametersVm();

            var currentUser = await _workContext.GetCurrentUser();
            var canManageOrder = (await _authorizationService.AuthorizeAsync(User, Policy.CanManageOrder)).Succeeded;
            search.CanManageOrder = canManageOrder;
            search.VendorId = currentUser.VendorId;
            search.HasOptions = false;
            var products = _searchProductService.GetProducts(search, param.Sort);
            var inStocks = products.Where(product => product.Stock > 0).OrderBy(product => product.Name);
            var outOfStocks = products.Where(product => product.Stock <= 0).OrderBy(product => product.Name);
            products = inStocks.Concat(outOfStocks);

            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            using (var csvWriter = new CsvWriter(writer))
            {
                csvWriter.WriteRecords(products);
                writer.Flush();
                var fileName = $"Products-{DateTime.Now.ToString("dd/MM/yyyy")}.csv";
                return File(stream.ToArray(), FileContentType.Binary, fileName);
            }
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

            await SaveProductMedias(model, product);
            AddOrDeleteProductOption(model.Product, product);
            AddOrDeleteProductAttribute(model, product);
            AddOrDeleteCategories(model, product);
            AddOrDeleteProductVariation(model.Product, product);
            AddOrDeleteProductLinks(model, product);

            product = _mapper.Map(model.Product, product, opt => opt.AfterMap((src, dest) => dest.UpdatedBy = currentUser));
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
            var childProducts = product.GetLinkedProducts(ProductLinkType.Super);

            foreach (var productVariationVm in productVm.Variations)
            {
                if (productVariationVm.Id == 0) // New variation added
                {
                    var productVariation = product.Clone();
                    productVariation.Name = productVariationVm.Name;
                    productVariation.SeoTitle = StringHelper.ToUrlFriendly(productVariationVm.Name);
                    productVariation.Price = productVariationVm.Price;
                    productVariation.OldPrice = productVariationVm.OldPrice;
                    productVariation.NormalizedName = productVariationVm.NormalizedName;
                    productVariation.HasOptions = false;
                    productVariation.IsVisibleIndividually = false;
                    productVariation.ThumbnailImage = product.ThumbnailImage;

                    foreach (var combinationVm in productVariationVm.OptionCombinations)
                    {
                        productVariation.AddOptionCombination(new ProductOptionCombination
                        {
                            OptionId = combinationVm.OptionId,
                            Value = combinationVm.Value,
                            SortIndex = combinationVm.SortIndex
                        });
                    }

                    product.AddProductLinks(new ProductLink
                    {
                        LinkType = ProductLinkType.Super,
                        Product = product,
                        LinkedProduct = productVariation
                    });
                }
                else
                {
                    var productVariation = childProducts.FirstOrDefault(x => x.Id == productVariationVm.Id);
                    productVariation.Price = productVariationVm.Price;
                    productVariation.OldPrice = productVariationVm.OldPrice;
                    productVariation.IsDeleted = false;

                    // Main product is renamed => Rename child products based on the new name
                    if (product.Name != productVm.Name)
                    {
                        productVariation.Name = productVariation.Name.Replace(product.Name, productVm.Name);
                        productVariation.SeoTitle = StringHelper.ToUrlFriendly(productVariation.Name);
                    }
                }
            }

            var removedVariations = product.ProductLinks
                .Where(item => item.LinkType == ProductLinkType.Super && !productVm.Variations.Any(v => v.Id == item.LinkedProduct.Id));
            foreach (var productLink in removedVariations)
            {
                _productLinkRepository.Remove(productLink);
                productLink.LinkedProduct.IsDeleted = true;
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
                product.ThumbnailImage = new Media { FileName = fileName };
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

            foreach (var productMediaId in model.Product.DeletedMediaIds)
            {
                var productMedia = product.Medias.First(x => x.Id == productMediaId);
                await _mediaService.DeleteMediaAsync(productMedia.Media);
                product.RemoveMedia(productMedia);
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
