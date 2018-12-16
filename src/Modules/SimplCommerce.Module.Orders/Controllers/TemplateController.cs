using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplCommerce.Module.Core.Extensions.Constants;

namespace SimplCommerce.Module.Orders.Controllers
{
    [Authorize(Policy.CanAccessDashboard)]
    [Route("template/orders")]
    [ApiController]
    public class TemplateController : Controller
    {
        private readonly IAuthorizationService _authorizationService;

        public TemplateController(IAuthorizationService authorizationService) => _authorizationService = authorizationService;

        [HttpGet("order-list")]
        public IActionResult OrderList() => User.IsInRole(RoleName.Seller) ? View("OrderListSeller") : View();

        [HttpGet("order-form")]
        public IActionResult OrderForm() => User.IsInRole(RoleName.Seller) ? View("OrderFormSeller") : View();
    }
}
