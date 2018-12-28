using SimplCommerce.Infrastructure.Models;

namespace SimplCommerce.Module.Payments.Models
{
    public class PaymentProvider : EntityBase
    {
        public PaymentProvider(long id) : base(id) { }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsEnabled { get; set; }

        public string ConfigureUrl { get; set; }

        public string LandingViewComponentName { get; set; }

        /// <summary>
        /// Additional setting for specific provider. Stored as json string.
        /// </summary>
        public string AdditionalSettings { get; set; }
    }
}
