using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplCommerce.Module.Core.Extensions.Constants;

namespace SimplCommerce.Module.Orders.Controllers
{
    [Authorize(Policy = Policy.CanManageOrder)]
    [Route("api/orders")]
    [ApiController]
    public class ImportApiController : Controller
    {
        
    }
}
