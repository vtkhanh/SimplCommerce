using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Catalog.Models;
using SimplCommerce.Module.Catalog.Services.Dtos;
using SimplCommerce.Module.Core.Services;

namespace SimplCommerce.Module.Catalog.Services
{
    public class ProductService : IProductService
    {
        private const long ProductEntityTypeId = 3;

        private readonly IMapper _mapper;
        private readonly IRepository<Product> _productRepo;
        private readonly IEntityService _entityService;

        public ProductService(IMapper mapper, IRepository<Product> productRepository, IEntityService entityService)
        {
            _mapper = mapper;
            _productRepo = productRepository;
            _entityService = entityService;
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
                .Where(i => !i.IsDeleted)
                .WhereIf(query.HasValue(), i => i.Name.Contains(query) || i.Sku.Contains(query))
                .OrderByDescending(i => i.HitCount) // Get the most frequently searched ones
                .TakeIf(maxItems.HasValue, maxItems.HasValue ? maxItems.Value : 0)
                .ToListAsync();

            if (products.Any()) 
            {
                foreach (var product in products)
                {
                    product.HitCount++;
                }
                await _productRepo.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }
    }
}
