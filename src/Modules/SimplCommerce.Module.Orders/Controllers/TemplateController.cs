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
        private readonly IAuthorizationService _authorizationService;
        private readonly IWorkContext _workContext;
        private readonly IOrderService _orderService;

        public TemplateController(IAuthorizationService authorizationService, IWorkContext workContext, IOrderService orderService) =>
            (_authorizationService, _workContext, _orderService) = (authorizationService, workContext, orderService);

        [HttpGet("order-list")]
        public IActionResult OrderList() => User.IsInRole(RoleName.Seller) ? View("OrderListSeller") : View();

        [HttpGet("order-form")]
        public IActionResult OrderForm() => User.IsInRole(RoleName.Seller) ? View("OrderFormSeller") : View();

        [HttpGet("order-edit/{id}")]
        public async Task<IActionResult> OrderFormEdit(int id)
        {
            if (User.IsInRole(RoleName.Admin))
            {
                return View();
            }

            var currentUser = await _workContext.GetCurrentUser();
            var orderCreatedById = await _orderService.GetOrderOwnerIdAsync(id);

            return currentUser.Id == orderCreatedById ? View("OrderFormSeller") : View("OrderFormReadOnly");
        }
    }
}
