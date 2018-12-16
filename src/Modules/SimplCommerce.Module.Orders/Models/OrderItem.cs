using System.ComponentModel.DataAnnotations.Schema;
using SimplCommerce.Infrastructure.Models;
using SimplCommerce.Module.Catalog.Models;

namespace SimplCommerce.Module.Orders.Models
{
    public class OrderItem : EntityBase
    {
        public Order Order { get; set; }

        public long ProductId { get; set; }

        public Product Product { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ProductPrice { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxPercent { get; set; }

        [NotMapped]
        public decimal SubTotal => Quantity * ProductPrice;

        [NotMapped]
        public decimal SubTotalCost => Quantity * Product.Cost;
    }
}
