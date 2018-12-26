using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Payments.Models;
using SimplCommerce.Module.Payments.Services;

namespace SimplCommerce.Module.Payments.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/payments-providers")]
    public class PaymentProviderApiController : Controller
    {
        private readonly IRepository<PaymentProvider> _paymentProviderRepository;
        private readonly IPaymentProviderService _paymentProviderService;

        public PaymentProviderApiController(IRepository<PaymentProvider> paymentProviderRepositor,
            IPaymentProviderService paymentProviderService)
        {
            _paymentProviderRepository = paymentProviderRepositor;
            _paymentProviderService = paymentProviderService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var providers = await _paymentProviderService.GetListAsync();
            return Json(providers);
        }

        [HttpPost("{id}/enable")]
        public async Task<IActionResult> Enable(long id)
        {
            var provider = await _paymentProviderRepository.Query().FirstOrDefaultAsync(x => x.Id == id);
            provider.IsEnabled = true;
            await _paymentProviderRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id}/disable")]
        public async Task<IActionResult> Disable(long id)
        {
            var provider = await _paymentProviderRepository.Query().FirstOrDefaultAsync(x => x.Id == id);
            provider.IsEnabled = false;
            await _paymentProviderRepository.SaveChangesAsync();
            return NoContent();
        }
    }
}
