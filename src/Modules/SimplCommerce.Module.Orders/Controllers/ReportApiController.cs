using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Extensions.Constants;
using SimplCommerce.Module.Orders.Services;
using SimplCommerce.Module.Orders.Services.Dtos;

namespace SimplCommerce.Module.Orders.Controllers
{
    [Authorize(Policy = Policy.CanAccessDashboard)]
    [Route("api/order-reports")]
    [ApiController]
    public class ReportApiController : Controller
    {
        private readonly IWorkContext _workContext;
        private readonly IReportService _reportService;

        public ReportApiController(IWorkContext workContext, IReportService reportService) =>
            (_workContext, _reportService) = (workContext, reportService);

        [HttpGet("revenue-report")]
        public async Task<IActionResult> GetRevenueReportAsync(long? createdById)
        {
            RevenueReportDto report;
            IList<object> series;

            if (User.IsInRole(RoleName.Admin))
            {
                report = await _reportService.GetRevenueReportAsync(DateTime.Now, createdById);
                series = new List<object>
                {
                    new { Name = "SubTotal", Data = report.SubTotals },
                    new { Name = "Total", Data = report.Totals },
                    new { Name = "Cost", Data = report.Costs },
                    new { Name = "Profit", Data = report.Profits }
                };
            }
            else
            {
                var currentUser = await _workContext.GetCurrentUser();
                report = await _reportService.GetRevenueReportBySellerAsync(DateTime.Now, currentUser.Id);
                series = new List<object>
                {
                    new { Name = "SubTotal", Data = report.SubTotals }
                };
            }

            var chartData = new
            {
                Title = "Revenue Report",
                report.Months,
                Series = series
            };

            return Json(chartData);
        }
    }
}
