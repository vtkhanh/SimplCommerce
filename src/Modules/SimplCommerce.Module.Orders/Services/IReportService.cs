using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimplCommerce.Module.Orders.Services
{
    public interface IReportService
    {
        Task<(IList<decimal>, IList<decimal>, IList<decimal>)> GetRevenueReport(DateTime time, long? createdById, int monthOffset = 3);
    }
}
