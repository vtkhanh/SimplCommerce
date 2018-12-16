using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplCommerce.Module.Core.Extensions.Constants;

namespace SimplCommerce.Module.Catalog.Controllers
{
    [Authorize(Policy.CanAccessDashboard)]
    [Route("template/products")]
    [ApiController]
    public class TemplateController : Controller
    {
        private readonly IAuthorizationService _authorizationService;

        public TemplateController(IAuthorizationService authorizationService) => _authorizationService = authorizationService;

        [HttpGet("product-list")]
        public IActionResult ProductList() => User.IsInRole(RoleName.Seller) ? View("ProductListSeller") : View();
    }
}
