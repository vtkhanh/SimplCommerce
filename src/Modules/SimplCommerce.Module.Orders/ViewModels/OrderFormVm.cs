using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using SimplCommerce.Infrastructure.Filters;
using SimplCommerce.Module.Orders.Models;

namespace SimplCommerce.Module.Orders.ViewModels
{
    public class OrderFormVm
    {
        public long OrderId { get; set; }

        public string ExternalId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Customer is required")]
        public long CustomerId { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public IList<SelectListItem> OrderStatusList =>
            Enum.GetValues(typeof(OrderStatus))
               .Cast<OrderStatus>()
               .Select(t => new SelectListItem
               {
                   Value = ((int)t).ToString(),
                   Text = t.ToString()
               }).ToList();

        [Required]
        [AtLeastItems(1, ErrorMessage = "Must select at least one product")]
        public IList<OrderItemVm> OrderItems { get; set; }

        public decimal ShippingAmount { get; set; }

        public decimal ShippingCost { get; set; }

        public decimal Discount { get; set; }

        public string TrackingNumber { get; set; }

        public long? PaymentProviderId { get; set; }

        public string Note { get; set; }

        public bool IsShopeeOrder { get; set; }
    }
}
