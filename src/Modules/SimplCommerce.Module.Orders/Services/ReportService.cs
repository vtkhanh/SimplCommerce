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

        public async Task<RevenueReportBuilder> GetRevenueReportAsync(DateTime time, long? createdById, int monthOffset = 3)
        {
            var report = await GetRevenueReportBuilderAsync(time, createdById, monthOffset);

            report.EvaluateSubTotals();
            report.EvaluateTotals();
            report.EvaluateCostsAndProfits();

            return report;
        }

        public async Task<RevenueReportBuilder> GetRevenueReportBySellerAsync(DateTime time, long sellerId, int monthOffset = 3)
        {
            var report = await GetRevenueReportBuilderAsync(time, sellerId, monthOffset);

            report.EvaluateSubTotals();

            return report;
        }

        private async Task<RevenueReportBuilder> GetRevenueReportBuilderAsync(DateTime time, long? createdById, int monthOffset)
        {
            var from = time.AddMonths((-1) * monthOffset);
            var to = time.AddMonths(monthOffset);

            var orders = await _orderRepo.QueryAsNoTracking()
                .WhereIf(createdById.HasValue && createdById > 0, order => order.CreatedById == createdById)
                .Where(order => (order.CompletedOn >= from || order.CompletedOn.Value.Month == from.Month) && (order.CompletedOn <= to || order.CompletedOn.Value.Month == to.Month))
                .Where(order => order.OrderStatus == OrderStatus.Complete)
                .ToListAsync();

            return new RevenueReportBuilder(orders);
        }

    }
}
