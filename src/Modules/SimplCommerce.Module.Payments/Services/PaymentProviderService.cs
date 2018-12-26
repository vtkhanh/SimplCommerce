using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Payments.Models;
using SimplCommerce.Module.Payments.ViewModels;

namespace SimplCommerce.Module.Payments.Services
{
    public class PaymentProviderService : IPaymentProviderService
    {
        private readonly IRepository<PaymentProvider> _paymentProviderRepo;

        public PaymentProviderService(IRepository<PaymentProvider> paymentProviderRepo) => 
            _paymentProviderRepo = paymentProviderRepo;

        public async Task<PaymentProviderVm> GetByIdAsync(long id)
        {
            var provider = await _paymentProviderRepo.QueryAsNoTracking().FirstOrDefaultAsync(item => item.Id == id);

            return provider != null ?
                new PaymentProviderVm
                {
                    Id = provider.Id,
                    Name = provider.Name,
                    IsEnabled = provider.IsEnabled,
                    ConfigureUrl = provider.ConfigureUrl,
                    LandingViewComponentName = provider.LandingViewComponentName
                }
                : null;
        }

        public async Task<IEnumerable<PaymentProviderVm>> GetListAsync(bool? isEnabled = null)
        {
            var providers = await _paymentProviderRepo.QueryAsNoTracking()
                .WhereIf(isEnabled.HasValue, item => item.IsEnabled == isEnabled)
                .Select(item => new PaymentProviderVm
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    IsEnabled = item.IsEnabled,
                    ConfigureUrl = item.ConfigureUrl,
                    LandingViewComponentName = item.LandingViewComponentName
                }).ToListAsync();

            return providers;
        }
    }
}
