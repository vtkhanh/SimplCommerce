using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimplCommerce.Infrastructure.Web.SmartTable;
using SimplCommerce.Module.Catalog.Models;
using SimplCommerce.Module.Catalog.ViewModels;

namespace SimplCommerce.Module.Catalog.Services
{
    public interface ISearchProductService
    {
        IQueryable<Product> BuildQuery(SearchProductParametersVm search);
        IEnumerable<ProductExportVm> GetProducts(SearchProductParametersVm search, Sort sort);
    }
}
