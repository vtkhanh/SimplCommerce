using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure;
using SimplCommerce.Module.Core.Data;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.Services;
using Xunit;

namespace SimplCommerce.Module.Orders.Tests
{
    public class OrderServiceTests
    {
        public class UpdateStatusAsync
        {

            private DbContextOptions<SimplDbContext> _options;

            public UpdateStatusAsync()
            {
                _options = new DbContextOptionsBuilder<SimplDbContext>()
                    .UseInMemoryDatabase(databaseName: nameof(UpdateStatusAsync))
                    .Options;

                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    var order = orderRepo.Query().FirstOrDefault();
                    if (order == null)
                    {
                        order = new Order() { OrderStatus = OrderStatus.Pending };
                        orderRepo.Add(order);
                        orderRepo.SaveChanges();
                    }
                }
            }

            [Theory]
            [InlineData(OrderStatus.Processing)]
            [InlineData(OrderStatus.Shipped)]
            [InlineData(OrderStatus.Paid)]
            [InlineData(OrderStatus.Complete)]
            [InlineData(OrderStatus.Cancelled)]
            public async Task UpdateStatusAsync_CanUpdateOrderStatus(OrderStatus status)
            {
                // Arrange
                long orderId;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    var order = await orderRepo.QueryAsNoTracking().FirstAsync();
                    orderId = order.Id;
                }

                // Action
                bool ok;
                string error;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    var orderService = new OrderService(orderRepo, null, null, null, null, null, null, null, null, null, null, null);
                    (ok, error) = await orderService.UpdateStatusAsync(orderId, status);
                }

                Order updated;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    updated = await orderRepo.QueryAsNoTracking().FirstAsync(item => item.Id == orderId);
                }

                // Assert
                Assert.True(ok);
                Assert.True(error.IsNullOrEmpty());
                Assert.Equal(status, updated.OrderStatus);

            }

            [Fact]
            public async Task UpdateTrackingNumberAsync_WithInvalidId_ShouldReturnError()
            {
                // Arrange
                Order order;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    order = await orderRepo.QueryAsNoTracking().FirstAsync();
                }

                // Action
                bool ok;
                string error;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    var orderService = new OrderService(orderRepo, null, null, null, null, null, null, null, null, null, null, null);
                    (ok, error) = await orderService.UpdateStatusAsync(order.Id + 1, OrderStatus.Complete);
                }

                // Assert
                Assert.False(ok);
                Assert.True(error.HasValue());
            }
        }
        public class UpdateTrackingNumberAsync
        {

            private DbContextOptions<SimplDbContext> _options;

            public UpdateTrackingNumberAsync()
            {
                _options = new DbContextOptionsBuilder<SimplDbContext>()
                    .UseInMemoryDatabase(databaseName: nameof(UpdateTrackingNumberAsync))
                    .Options;

                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    var order = orderRepo.Query().FirstOrDefault();
                    if (order == null)
                    {
                        order = new Order() { TrackingNumber = "123456789097" };
                        orderRepo.Add(order);
                        orderRepo.SaveChanges();
                    }
                }
            }

            [Fact]
            public async Task UpdateTrackingNumberAsync_CanUpdateTrackingNumber()
            {
                // Arrange
                var trackingNumber = "840242824093";
                long orderId;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    var order = await orderRepo.QueryAsNoTracking().FirstAsync();
                    orderId = order.Id;
                }

                // Action
                bool ok;
                string error;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    var orderService = new OrderService(orderRepo, null, null, null, null, null, null, null, null, null, null, null);
                    (ok, error) = await orderService.UpdateTrackingNumberAsync(orderId, trackingNumber);
                }

                Order updated;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    updated = await orderRepo.QueryAsNoTracking().FirstAsync(item => item.Id == orderId);
                }

                // Assert
                Assert.True(ok);
                Assert.True(error.IsNullOrEmpty());
                Assert.Equal(trackingNumber, updated.TrackingNumber);
            }

            [Fact]
            public async Task UpdateTrackingNumberAsync_WithInvalidId_ShouldReturnError()
            {
                // Arrange
                var trackingNumber = "840242824093";
                Order order;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    order = await orderRepo.QueryAsNoTracking().FirstAsync();
                }

                // Action
                bool ok;
                string error;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    var orderService = new OrderService(orderRepo, null, null, null, null, null, null, null, null, null, null, null);
                    (ok, error) = await orderService.UpdateTrackingNumberAsync(order.Id + 1, trackingNumber);
                }

                // Assert
                Assert.False(ok);
                Assert.True(error.HasValue());
            }
        }

    }
}
