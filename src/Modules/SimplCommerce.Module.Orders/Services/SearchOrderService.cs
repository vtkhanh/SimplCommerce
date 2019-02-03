using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Web.SmartTable;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.ViewModels;

namespace SimplCommerce.Module.Orders.Services
{
    public class SearchOrderService : ISearchOrderService
    {
        private readonly IRepository<Order> _orderRepo;

        public SearchOrderService(IRepository<Order> orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public IQueryable<Order> BuildQuery(SearchOrderParametersVm search) => BuildQuery(search, null);

        public async Task<IEnumerable<OrderExportVm>> GetOrdersAsync(SearchOrderParametersVm search, Sort sort)
        {
            const string DateFormat = "dd/MM/yyyy";

            if (sort == null || !sort.Predicate.HasValue())
            {
                sort = new Sort() { Predicate = "Id", Reverse = true };
            }

            var query = BuildQuery(search, sort);

            var orders = await query.Select(order => new OrderExportVm
            {
                Id = order.Id,
                CustomerName = order.Customer.FullName,
                CreatedBy = order.CreatedBy.FullName,
                TrackingNumber = order.TrackingNumber,
                Status = order.OrderStatus,
                CreatedOn = order.CreatedOn.ToString(DateFormat),
                CompletedOn = order.CompletedOn.HasValue ? order.CompletedOn.Value.ToString(DateFormat) : "",
                Cost = order.OrderTotalCost,
                Total = order.OrderTotal
            }).ToListAsync();

            return orders;
        }

        private IQueryable<Order> BuildQuery(SearchOrderParametersVm search, Sort sort)
        {
            var query = _orderRepo.QueryAsNoTracking()
                .Include(i => i.Customer)
                .Include(i => i.CreatedBy)
                .WhereIf(!search.CanManageOrder, i => i.VendorId == search.UserVendorId)
                .WhereIf(search.Id.HasValue, i => i.Id == search.Id)
                .WhereIf(search.Status.HasValue, i => i.OrderStatus == search.Status)
                .WhereIf(search.CustomerName.HasValue(), i => i.Customer.FullName.Contains(search.CustomerName))
                .WhereIf(search.TrackingNumber.HasValue(), i => i.TrackingNumber.Contains(search.TrackingNumber))
                .WhereIf(search.CreatedBy.HasValue(), i => i.CreatedBy.FullName.Contains(search.CreatedBy))
                .WhereIf(search.CreatedBefore.HasValue, i => i.CreatedOn <= search.CreatedBefore)
                .WhereIf(search.CreatedAfter.HasValue, i => i.CreatedOn >= search.CreatedAfter)
                .WhereIf(search.CompletedBefore.HasValue, x => x.CompletedOn <= search.CompletedBefore)
                .WhereIf(search.CompletedAfter.HasValue, x => x.CompletedOn >= search.CompletedAfter);

            if (sort != null && sort.Predicate.HasValue())
            {
                query = query.OrderByName(sort.Predicate, sort.Reverse);
            }

            return query;
        }
            
    }
}
