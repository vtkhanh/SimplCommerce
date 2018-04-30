using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SimplCommerce.Infrastructure.Filters;
using SimplCommerce.Module.Orders.Models;

namespace SimplCommerce.Module.Orders.ViewModels
{
    public class OrderFormVm
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Customer is required")]
        public long CustomerId { get; set;}

        public OrderStatus OrderStatus { get; set;}

        public string OrderStatusDisplay { get; set; }

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