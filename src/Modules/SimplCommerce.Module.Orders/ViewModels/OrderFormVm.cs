using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimplCommerce.Module.Orders.ViewModels
{
    public class OrderFormVm
    {
        [Required]
        public int CustomerId { get; set;}

        public IList<OrderItemVm> OrderItems { get; set;}
        
        public decimal ShippingAmount { get; set; }

        public decimal Discount { get; set; }

        public decimal SubTotal { get; set; }

        public decimal OrderTotal { get; set; }
    }
}