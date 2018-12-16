using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using SimplCommerce.Infrastructure.Filters;
using SimplCommerce.Module.Orders.Models;

namespace SimplCommerce.Module.Orders.ViewModels
{
    public class GetOrderVm
    {
        public long OrderId { get; set; }

        public long CustomerId { get; set; }

        public long CreatedById { get; set; }

        public long? VendorId { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public IList<SelectListItem> OrderStatusList =>
            Enum.GetValues(typeof(OrderStatus))
               .Cast<OrderStatus>()
               .Select(t => new SelectListItem
               {
                   Value = ((int)t).ToString(),
                   Text = t.ToString()
               }).ToList();

        public string OrderStatusDisplay { get; set; }

        public IList<OrderItemVm> OrderItems { get; set; }

        public decimal ShippingAmount { get; set; }

        public decimal ShippingCost { get; set; }

        public decimal Discount { get; set; }

        public string TrackingNumber { get; set; }

        public decimal SubTotal { get; set; }

        public decimal SubTotalCost { get; set; }

        public decimal OrderTotal { get; set; }

        public decimal OrderTotalCost { get; set; }
    }
}
