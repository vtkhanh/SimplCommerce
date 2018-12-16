using System.ComponentModel.DataAnnotations.Schema;
using SimplCommerce.Infrastructure.Models;
using SimplCommerce.Module.Core.Models;

namespace SimplCommerce.Module.ShippingTableRate.Models
{
    public class PriceAndDestination : EntityBase
    {
        public Country Country { get; set; }

        public long? CountryId { get; set; }

        public StateOrProvince StateOrProvince { get; set; }

        public long? StateOrProvinceId { get; set; }

        public string Note { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MinOrderSubtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingPrice { get; set; }
    }
}
