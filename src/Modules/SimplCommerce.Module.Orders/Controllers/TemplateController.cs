using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Extensions.Constants;
using SimplCommerce.Module.Orders.Services;

namespace SimplCommerce.Module.Orders.Controllers
{
    [Authorize(Policy.CanAccessDashboard)]
    [Route("template/orders")]
    [ApiController]
    public class TemplateController : Controller
    {
        private const string OrderListSellerView = "OrderListSeller";
        private const string OrderFormView = "OrderForm";
        private const string OrderFormSellerView = "OrderFormSeller";
        private const string OrderFormRestrictedView = "OrderFormRestricted";

        private readonly IWorkContext _workContext;
        private readonly IOrderService _orderService;

        public TemplateController(IWorkContext workContext, IOrderService orderService) =>
            (_workContext, _orderService) = (workContext, orderService);

        [HttpGet("order-list")]
        public IActionResult OrderList() => User.IsInRole(RoleName.Seller) ? View(OrderListSellerView) : View();

        [HttpGet("order-create")]
        public IActionResult OrderFormCreate() => User.IsInRole(RoleName.Seller) ? View(OrderFormSellerView) : View(OrderFormView);

        [HttpGet("order-edit/{id}")]
        public async Task<IActionResult> OrderFormEdit(int id)
        {
            if (User.IsInRole(RoleName.Admin))
            {
                return View(OrderFormView);
            }

            var currentUser = await _workContext.GetCurrentUser();
            var orderCreatedById = await _orderService.GetOrderOwnerIdAsync(id);

            return currentUser.Id == orderCreatedById ? View(OrderFormSellerView) : View(OrderFormRestrictedView);
        }
    }
}
