using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using SimplCommerce.Infrastructure.Models;
using SimplCommerce.Module.Core.Models;

namespace SimplCommerce.Module.ShoppingCart.Models
{
    public class Cart : EntityBase
    {
        public Cart()
        {
            CreatedOn = DateTimeOffset.Now;
            IsActive = true;
        }

        public long UserId { get; set; }

        public User User { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset? UpdatedOn { get; set; }

        public bool IsActive { get; set; }

        public string CouponCode { get; set; }

        public string CouponRuleName { get; set; }

        public string ShippingMethod { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ShippingAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TaxAmount { get; set; }

        public IList<CartItem> Items { get; set; } = new List<CartItem>();

        /// <summary>
        /// Json serialized of shipping form
        /// </summary>
        public string ShippingData { get; set; }
    }
}
