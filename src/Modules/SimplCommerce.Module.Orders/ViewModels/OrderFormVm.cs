using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SimplCommerce.Infrastructure.Filters;

namespace SimplCommerce.Module.Orders.ViewModels
{
    public class OrderFormVm
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Customer is required")]
        public int CustomerId { get; set;}

        [Required]
        [AtLeastItems(1, ErrorMessage = "Must select at least one product")]
        public IList<OrderItemVm> OrderItems { get; set;}
        
        public decimal ShippingAmount { get; set; }

        public decimal Discount { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "SubTotal is required")]
        public decimal SubTotal { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "OrderTotal is required")]
        public decimal OrderTotal { get; set; }
    }
}