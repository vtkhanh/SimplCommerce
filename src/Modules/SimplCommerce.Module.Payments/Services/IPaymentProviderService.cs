using System.Collections.Generic;
using System.Threading.Tasks;
using SimplCommerce.Module.Payments.ViewModels;

namespace SimplCommerce.Module.Payments.Services
{
    public interface IPaymentProviderService
    {
        Task<IEnumerable<PaymentProviderVm>> GetListAsync(bool? isEnabled = null);

        Task<PaymentProviderVm> GetByIdAsync(long id);

        Task<(bool, string)> ToggleAsync(long id, bool isEnabled);
    }
}
