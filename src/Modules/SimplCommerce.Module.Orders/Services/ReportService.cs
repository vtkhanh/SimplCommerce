using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Orders.Models;

namespace SimplCommerce.Module.Orders.Services
{
    public class ReportService : IReportService
    {
        private readonly IRepository<Order> _orderRepo;

        public ReportService(IRepository<Order> orderRepo) => _orderRepo = orderRepo;

        public async Task<(IList<decimal>, IList<decimal>, IList<decimal>)> GetRevenueReport(DateTime time, long? createdById, int monthOffset = 3)
        {
            var from = time.AddMonths((-1) * monthOffset);
            var to = time.AddMonths(monthOffset);

            var orders = await _orderRepo.QueryAsNoTracking()
                .WhereIf(createdById.HasValue, order => order.CreatedById == createdById)
                .Where(order => order.CreatedOn >= from && order.CreatedOn <= to)
                .ToListAsync();
            
            throw new NotImplementedException();
        }
    }
}
