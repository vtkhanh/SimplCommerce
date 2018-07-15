using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Catalog.Models;
using SimplCommerce.Module.Catalog.Services.Dtos;
using SimplCommerce.Module.Core.Extensions.Constants;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Core.Services;

namespace SimplCommerce.Module.Catalog.Services
{
    public class ProductService : IProductService
    {
        private const long ProductEntityTypeId = 3;

        private readonly IMapper _mapper;
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<AppSetting> _appSettingRepo;
        private readonly IEntityService _entityService;
        private readonly IMediaService _mediaService;

        public ProductService(IMapper mapper,
            IRepository<Product> productRepo, IRepository<AppSetting> appSettingRepo,
            IEntityService entityService, IMediaService mediaService)
        {
            _mapper = mapper;
            _productRepo = productRepo;
            _appSettingRepo = appSettingRepo;
            _entityService = entityService;
            _mediaService = mediaService;
        }

        public void Create(Product product)
        {
            using (var transaction = _productRepo.BeginTransaction())
            {
                product.SeoTitle = _entityService.ToSafeSlug(product.SeoTitle, product.Id, ProductEntityTypeId);
                _productRepo.Add(product);
                _productRepo.SaveChanges();

                _entityService.Add(product.Name, product.SeoTitle, product.Id, ProductEntityTypeId);
                _productRepo.SaveChanges();

                transaction.Commit();
            }
        }

        public void Update(Product product)
        {
            var slug = _entityService.Get(product.Id, ProductEntityTypeId);
            if (product.IsVisibleIndividually)
            {
                product.SeoTitle = _entityService.ToSafeSlug(product.SeoTitle, product.Id, ProductEntityTypeId);
                if (slug != null)
                {
                    _entityService.Update(product.Name, product.SeoTitle, product.Id, ProductEntityTypeId);
                }
                else
                {
                    _entityService.Add(product.Name, product.SeoTitle, product.Id, ProductEntityTypeId);
                }
            }
            else
            {
                if (slug != null)
                {
                    _entityService.Remove(product.Id, ProductEntityTypeId);
                }
            }
            _productRepo.SaveChanges();
        }

        public async Task Delete(Product product)
        {
            product.IsDeleted = true;
            await _entityService.Remove(product.Id, ProductEntityTypeId);
            _productRepo.SaveChanges();
        }

        public async Task<IEnumerable<ProductDto>> Search(string query, int? maxItems = null)
        {
            var products = await _productRepo.Query()
                .Include(i => i.ThumbnailImage)
                .Where(i => !i.IsDeleted)
                .WhereIf(query.HasValue(), i => i.Name.Contains(query) || i.Sku.Contains(query))
                .OrderByDescending(i => i.HitCount) // Get the most frequently searched ones
                .TakeIf(maxItems.HasValue, maxItems.HasValue ? maxItems.Value : 0)
                .ToListAsync();

            if (products.Any() && query.HasValue())
            {
                foreach (var product in products)
                {
                    product.HitCount++;
                }
                await _productRepo.SaveChangesAsync();
            }

            var result = products.Select(item =>
                _mapper.Map<Product, ProductDto>(item,
                    opt => opt.AfterMap((src, dest) => dest.ThumbnailImageUrl = _mediaService.GetThumbnailUrl(src.ThumbnailImage))));

            return result;
        }

        public async Task<ProductSettingDto> GetProductSetting()
        {
            var settings = await _appSettingRepo.Query()
                .Where(x => x.IsVisibleInCommonSettingPage && x.Module == "Catalog")
                .ToListAsync();

            var result = new ProductSettingDto()
            {
                ConversionRate = ParseDecimalString(settings, AppSettingKey.CurrencyConversionRate, 1),
                FeeOfPicker = ParseDecimalString(settings, AppSettingKey.FeeOfPicker, 0),
                FeePerWeightUnit = ParseDecimalString(settings, AppSettingKey.FeePerWeightUnit, 0)
            };

            return result;
        }

        public async Task<(bool, string)> AddStock(string barcode) 
        {
            var product = await _productRepo.Query().FirstOrDefaultAsync(item => item.Sku == barcode);
            if (product == null) return (false, $"No product found with barcode: {barcode}");

            // +1 to current stock
            product.Stock++;

            await _productRepo.SaveChangesAsync();
            
            return (true, "");
        }

        private decimal ParseDecimalString(IList<AppSetting> settings, string key, decimal defaultVal)
        {
            var value = settings.FirstOrDefault(i => i.Key == key)?.Value;
            var ok = decimal.TryParse(value, out decimal result);
            return ok ? result : defaultVal;
        }
    }
}
