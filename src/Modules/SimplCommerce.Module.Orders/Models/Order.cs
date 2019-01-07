using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using SimplCommerce.Infrastructure.Models;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Payments.Models;

namespace SimplCommerce.Module.Orders.Models
{
    public class Order : EntityBase
    {
        public Order()
        {
            CreatedOn = DateTimeOffset.Now;
            OrderStatus = OrderStatus.Pending;
        }

        public Order(long id) : base(id) { }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset? UpdatedOn { get; set; }

        public DateTimeOffset? CompletedOn { get; set; }

        public long CreatedById { get; set; }

        public User CreatedBy { get; set; }

        public long CustomerId { get; set; }

        public virtual User Customer { get; set; }

        public long? VendorId { get; set; }

        public string CouponCode { get; set; }

        public string CouponRuleName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }

        public long? ShippingAddressId { get; set; }

        public OrderAddress ShippingAddress { get; set; }

        public long? BillingAddressId { get; set; }

        public OrderAddress BillingAddress { get; set; }

        public IList<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public OrderStatus OrderStatus { get; set; }

        public long? ParentId { get; set; }

        public Order Parent { get; set; }

        public string ShippingMethod { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingCost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal OrderTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal OrderTotalCost { get; set; }

        public long? PaymentProviderId { get; set; }

        [ForeignKey("PaymentProviderId")]
        public PaymentProvider PaymentProvider { get; set; }

        public string PaymentMethod { get; set; }

        public string TrackingNumber { get; set; }

        public string Note { get; set; }

        public IList<Order> Children { get; protected set; } = new List<Order>();

        public void AddOrderItem(OrderItem item)
        {
            item.Order = this;
            OrderItems.Add(item);
        }
    }
}
