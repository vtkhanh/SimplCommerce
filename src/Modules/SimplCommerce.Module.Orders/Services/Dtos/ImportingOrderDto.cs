using System;
using SimplCommerce.Module.Orders.Models;

namespace SimplCommerce.Module.Orders.Services.Dtos
{
    internal class ImportingOrderDto
    {
        public string ExternalOrderId { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTimeOffset? OrderedDate { get; set; }
        public string Note { get; set; }
        public string TrackingNumber { get; set; }
        public OrderStatus Status { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal ShippingCost { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string ShippingAddress { get; set; }
        public decimal OrderTotal { get; set; }
    }
}
