using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.Services.Dtos;

namespace SimplCommerce.Module.Orders.Services
{
    public class ReportService : IReportService
    {
        private readonly IRepository<Order> _orderRepo;

        public ReportService(IRepository<Order> orderRepo) => _orderRepo = orderRepo;

        public async Task<RevenueReportDto> GetRevenueReportAsync(DateTime time, long? createdById, int monthOffset = 3)
        {
            var from = time.AddMonths((-1) * monthOffset);
            var to = time.AddMonths(monthOffset);

            List<Order> orders = await GetCompleteOrdersAsync(createdById, from, to);
            var report = new RevenueReportDto(from, to);
            report.AddSubTotals(orders);
            report.AddTotals(orders);
            report.AddCostsAndProfits(orders);

            return report;
        }

        public async Task<RevenueReportDto> GetRevenueReportBySellerAsync(DateTime time, long sellerId, int monthOffset = 3)
        {
            var from = time.AddMonths((-1) * monthOffset);
            var to = time.AddMonths(monthOffset);

            var orders = await GetCompleteOrdersAsync(sellerId, from, to);
            var report = new RevenueReportDto(from, to);
            report.AddSubTotals(orders);

            return report;
        }

        private async Task<List<Order>> GetCompleteOrdersAsync(long? createdById, DateTime from, DateTime to)
        {
            return await _orderRepo.QueryAsNoTracking()
                .WhereIf(createdById.HasValue && createdById > 0, order => order.CreatedById == createdById)
                .Where(order => order.CompletedOn >= from && order.CompletedOn <= to)
                .Where(order => order.OrderStatus == OrderStatus.Complete)
                .ToListAsync();
        }

    }
}
