using System;
using System.Threading.Tasks;

namespace SimplCommerce.Module.Orders.Services
{
    public interface IReportService
    {
        Task<RevenueReportBuilder> GetRevenueReportAsync(DateTime time, long? createdById, int monthOffset = 3);
        Task<RevenueReportBuilder> GetRevenueReportBySellerAsync(DateTime time, long sellerId, int monthOffset = 3);
    }
}
