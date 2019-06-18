using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Core.Extensions.Constants;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Orders.Models;

namespace SimplCommerce.Module.Orders.Services
{
    public class ReportService : IReportService
    {
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<AppSetting> _appSettingRepo;

        public ReportService(IRepository<Order> orderRepo, IRepository<AppSetting> appSettingRepo)
        {
            _orderRepo = orderRepo;
            _appSettingRepo = appSettingRepo;
        }

        public async Task<RevenueReportBuilder> GetRevenueReportAsync(DateTime time, long? createdById)
        {
            var report = await GetRevenueReportBuilderAsync(time, createdById);

            report.EvaluateSubTotals();
            report.EvaluateTotals();
            report.EvaluateCostsAndProfits();

            return report;
        }

        public async Task<RevenueReportBuilder> GetRevenueReportBySellerAsync(DateTime time, long sellerId)
        {
            var report = await GetRevenueReportBuilderAsync(time, sellerId);

            report.EvaluateSubTotals();

            return report;
        }

        private async Task<RevenueReportBuilder> GetRevenueReportBuilderAsync(DateTime time, long? createdById)
        {
            var monthOffsetSetting = await _appSettingRepo.QueryAsNoTracking().FirstOrDefaultAsync(setting => setting.Key == AppSettingKey.ReportMonthOffset);
            var monthOffset = int.TryParse(monthOffsetSetting?.Value, out int offset) ? offset : 0;

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
