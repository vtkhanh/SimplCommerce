using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimplCommerce.Module.Core.Services;

namespace SimplCommerce.Module.Core.Controllers
{
    [Route("api/customer")]
    public class CustomerApiController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerApiController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [Authorize(Roles = "admin, vendor")]
        [HttpGet("search")]
        public async Task<IActionResult> Search(string query)
        {
            try
            {
                var customers = await _customerService.SearchAsync(query);
                return Ok(customers);
            }
            catch (Exception exception)
            {
                return BadRequest(new { Error = exception.Message });
            }
        }
    }
}