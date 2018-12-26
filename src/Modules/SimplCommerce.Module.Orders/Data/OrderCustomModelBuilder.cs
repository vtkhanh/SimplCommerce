using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Orders.Models;

namespace SimplCommerce.Module.Orders.Data
{
    public class OrderCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderAddress>(x =>
            {
                x.HasOne(d => d.District)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Restrict);

                x.HasOne(d => d.StateOrProvince)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Restrict);

                x.HasOne(d => d.Country)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Order>(order =>
            {
                order.HasOne(x => x.ShippingAddress)
                    .WithMany()
                    .HasForeignKey(x => x.ShippingAddressId)
                    .OnDelete(DeleteBehavior.Restrict);

                order.HasOne(x => x.BillingAddress)
                    .WithMany()
                    .HasForeignKey(x => x.BillingAddressId)
                    .OnDelete(DeleteBehavior.Restrict);

                order.HasOne(i => i.Customer)
                    .WithMany()
                    .HasForeignKey(i => i.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
