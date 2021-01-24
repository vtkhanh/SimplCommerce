using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplCommerce.Module.Core.Extensions.Constants;

namespace SimplCommerce.Module.Localization.Controllers
{
    [Authorize(Policy.CanAccessDashboard)]
    [Route("template/localization")]
    [ApiController]
    public class TemplateController : Controller
    {
        private const string TranslationListView = "TranslationList";

        [HttpGet("translation-list")]
        public IActionResult Get() => View(TranslationListView);
    }
}
