using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SimplCommerce.Infrastructure;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Web.SmartTable;
using SimplCommerce.Module.Catalog.Models;
using SimplCommerce.Module.Catalog.ViewModels;

namespace SimplCommerce.Module.Catalog.Services
{
    public class SearchProductService : ISearchProductService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Product> _productRepo;

        public SearchProductService(IMapper mapper, IRepository<Product> productRepo) => (_mapper, _productRepo) = (mapper, productRepo);

        public IQueryable<Product> BuildQuery(SearchProductParametersVm search) => BuildQuery(search, null);

        public IEnumerable<ProductExportVm> GetProducts(SearchProductParametersVm search, Sort sort)
        {
            if (sort == null || !sort.Predicate.HasValue())
            {
                sort = new Sort() { Predicate = "Id", Reverse = true };
            }

            var query = BuildQuery(search, sort);

            var products = query.Select(_mapper.Map<ProductExportVm>);

            return products;
        }

        private IQueryable<Product> BuildQuery(SearchProductParametersVm search, Sort sort)
        {
            var query = _productRepo.QueryAsNoTracking()
                .Where(item => !item.IsDeleted)
                .WhereIf(!search.CanManageOrder, x => x.VendorId == search.VendorId)
                .WhereIf(search.Name.HasValue(), x => x.Name.Contains(search.Name))
                .WhereIf(search.Sku.HasValue(), x => x.Sku.Contains(search.Sku))
                .WhereIf(search.HasOptions.HasValue, x => x.HasOptions == search.HasOptions)
                .WhereIf(search.InStock.HasValue, x => (search.InStock.Value && x.Stock > 0) || (!search.InStock.Value && x.Stock <= 0))
                .WhereIf(search.IsVisibleIndividually.HasValue, x => x.IsVisibleIndividually == search.IsVisibleIndividually)
                .WhereIf(search.IsPublished.HasValue, x => x.IsPublished == search.IsPublished)
                .WhereIf(search.CreatedBefore.HasValue, x => x.CreatedOn <= search.CreatedBefore)
                .WhereIf(search.CreatedAfter.HasValue, x => x.CreatedOn >= search.CreatedAfter)
                ;

            if (sort != null && sort.Predicate.HasValue())
            {
                query = query.OrderByName(sort.Predicate, sort.Reverse);
            }

            return query;
        }
    }
}
