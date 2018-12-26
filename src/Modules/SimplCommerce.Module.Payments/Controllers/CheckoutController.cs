using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Payments.Models;
using SimplCommerce.Module.Payments.ViewModels;

namespace SimplCommerce.Module.Payments.Controllers
{
    [Route("checkout")]
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly IRepository<PaymentProvider> _paymentProviderRepository;
        //private readonly IRepository<Cart> _cartRepository;
        private readonly IWorkContext _workContext;

        public CheckoutController(IRepository<PaymentProvider> paymentProviderRepository,
            //IRepository<Cart> cartRepository,
            IWorkContext workContext)
        {
            _paymentProviderRepository = paymentProviderRepository;
            //_cartRepository = cartRepository;
            _workContext = workContext;
        }

        [HttpGet("payment")]
        public IActionResult Payment()
        {
            return Redirect("~/");
            // TODO: Move this code to Order module
            //var currentUser = await _workContext.GetCurrentUser();
            //var cart = _cartRepository.Query().FirstOrDefault(x => x.UserId == currentUser.Id && x.IsActive);
            //if(cart == null)
            //{
            //    return Redirect("~/");
            //}

            //var checkoutPaymentForm = new CheckoutPaymentForm();
            //checkoutPaymentForm.PaymentProviders = await _paymentProviderRepository.Query()
            //    .Where(x => x.IsEnabled)
            //    .Select(x => new PaymentProviderVm
            //    {
            //        Id = x.Id,
            //        Name = x.Name,
            //        LandingViewComponentName = x.LandingViewComponentName
            //    }).ToListAsync();

            //return View(checkoutPaymentForm);
        }
    }
}
