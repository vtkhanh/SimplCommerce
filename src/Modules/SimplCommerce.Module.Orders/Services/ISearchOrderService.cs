using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.ViewModels;

namespace SimplCommerce.Module.Orders.Services
{
    public interface ISearchOrderService
    {
        IQueryable<Order> BuildQuery(SearchParametersVm search);
        Task<IList<OrderExportVm>> GetOrdersAsync(SearchParametersVm search);
    }
}
