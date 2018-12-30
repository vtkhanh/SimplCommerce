using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplCommerce.Module.Core.Extensions.Constants;
using SimplCommerce.Module.Orders.Services;

namespace SimplCommerce.Module.Orders.Controllers
{
    [Authorize(Policy = Policy.CanAccessDashboard)]
    [Route("api/order-reports")]
    [ApiController]
    public class ReportApiController : Controller
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IReportService _reportService;

        public ReportApiController(IAuthorizationService authorizationService, IReportService reportService) =>
            (_authorizationService, _reportService) = (authorizationService, reportService);

        [HttpGet("revenue-report")]
        public IActionResult GetRevenueReport(DateTime time)
        {
            return null;
        }
    }
}
