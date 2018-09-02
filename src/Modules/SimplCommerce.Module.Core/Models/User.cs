using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using SimplCommerce.Infrastructure.Models;

namespace SimplCommerce.Module.Core.Models
{
    public class User : IdentityUser<long>, IEntityWithTypedId<long>
    {
        public User()
        {
            CreatedOn = DateTimeOffset.Now;
            UpdatedOn = DateTimeOffset.Now;
        }

        public Guid UserGuid { get; set; }

        public string FullName { get; set; }

        public long? VendorId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public virtual ICollection<UserAddress> UserAddresses { get; set; } = new HashSet<UserAddress>();

        public virtual Address DefaultShippingAddress { get; set; }

        public long? DefaultShippingAddressId { get; set; }

        public virtual Address DefaultBillingAddress { get; set; }

        public long? DefaultBillingAddressId { get; set; }

        public virtual ICollection<UserRole> Roles { get; set; } =  new HashSet<UserRole>();

        public virtual ICollection<UserCustomerGroup> CustomerGroups { get; set; } = new HashSet<UserCustomerGroup>();
    }
}
