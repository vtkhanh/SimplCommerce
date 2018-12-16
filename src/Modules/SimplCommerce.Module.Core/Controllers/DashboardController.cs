using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SimplCommerce.Module.Core.Controllers
{
    [Authorize(Roles = "admin, vendor, seller")]
    public class DashboardController : Controller
    {
        [Route("admin/dashboard")]
        public IActionResult HomeTemplate()
        {
            return View();
        }
    }
}
