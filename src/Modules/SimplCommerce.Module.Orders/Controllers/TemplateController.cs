using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Extensions.Constants;
using SimplCommerce.Module.Orders.Services;
using SimplCommerce.Module.Orders.ViewModels;

namespace SimplCommerce.Module.Orders.Controllers
{
    [Authorize(Policy.CanAccessDashboard)]
    [Route("template/orders")]
    [ApiController]
    public class TemplateController : Controller
    {
        private const string OrderListView = "OrderList";
        private const string OrderListSellerView = "OrderListSeller";
        private const string OrderFormView = "OrderForm";
        private const string OrderFormSellerView = "OrderFormSeller";
        private const string OrderFormRestrictedView = "OrderFormRestricted";
        private const string OrderReportView = "OrderReport";
        private const string OrderReportSellerView = "OrderReportSeller";

        private readonly IWorkContext _workContext;
        private readonly IOrderService _orderService;

        public TemplateController(IWorkContext workContext, IOrderService orderService) =>
            (_workContext, _orderService) = (workContext, orderService);

        [HttpGet("order-list")]
        public IActionResult GetOrderList() => User.IsInRole(RoleName.Admin) ? View(OrderListView) : View(OrderListSellerView);

        [HttpGet("order-create")]
        public IActionResult GetOrderFormCreate() => User.IsInRole(RoleName.Admin) ? View(OrderFormView) : View(OrderFormSellerView);

        [HttpGet("order-edit/{id}")]
        public async Task<IActionResult> GetOrderFormEdit(int id)
        {
            if (User.IsInRole(RoleName.Admin))
            {
                return View(OrderFormView);
            }

            var currentUser = await _workContext.GetCurrentUser();
            var orderCreatedById = await _orderService.GetOrderOwnerIdAsync(id);

            return currentUser.Id == orderCreatedById ? View(OrderFormSellerView) : View(OrderFormRestrictedView);
        }

        [HttpGet("order-report")]
        public async Task<IActionResult> GetOrderReport()
        {
            if (User.IsInRole(RoleName.Admin))
            {
                return View(OrderReportView);
            }
            var currentUser = await _workContext.GetCurrentUser();
            return View(OrderReportSellerView, new OrderReportSellerVm { UserName = currentUser.FullName });
        }
    }
}
