using System;
using System.Threading.Tasks;
using SimplCommerce.Module.Orders.Services.Dtos;

namespace SimplCommerce.Module.Orders.Services
{
    public interface IReportService
    {
        Task<RevenueReportDto> GetRevenueReportAsync(DateTime time, long? createdById, int monthOffset = 3);
        Task<RevenueReportDto> GetRevenueReportBySellerAsync(DateTime time, long sellerId, int monthOffset = 3);
    }
}
