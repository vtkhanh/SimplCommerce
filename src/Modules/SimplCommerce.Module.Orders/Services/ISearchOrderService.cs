using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimplCommerce.Infrastructure.Web.SmartTable;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.ViewModels;

namespace SimplCommerce.Module.Orders.Services
{
    public interface ISearchOrderService
    {
        IQueryable<Order> BuildQuery(SearchOrderParametersVm search);
        Task<IEnumerable<OrderExportVm>> GetOrdersAsync(SearchOrderParametersVm search, Sort sort);
    }
}
