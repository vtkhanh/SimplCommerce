using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using SimplCommerce.Infrastructure;
using SimplCommerce.Module.Catalog.Models;
using SimplCommerce.Module.Core.Data;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.Services;
using SimplCommerce.Module.Orders.ViewModels;
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
            public async Task CanUpdateOrderStatus(OrderStatus status)
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
            public async Task WithInvalidId_ShouldReturnError()
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
            public async Task CanUpdateTrackingNumber()
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
            public async Task WithInvalidId_ShouldReturnError()
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

        public class CreateOrderAsync
        {

            private DbContextOptions<SimplDbContext> _options;

            private void Init(string dbName)
            {
                _options = new DbContextOptionsBuilder<SimplDbContext>()
                    .UseInMemoryDatabase(databaseName: dbName)
                    .Options;

                using (var context = new SimplDbContext(_options))
                {
                    var products = new List<Product>{
                        new Product() { Stock = 10 },
                        new Product() { Stock = 20 },
                    };
                    var productRepo = new Repository<Product>(context);
                    productRepo.AddRange(products);
                    productRepo.SaveChanges();
                }
            }

            [Fact]
            public async Task CanCreateOrder_IncludingSubTotal_AndOrderTotal_AndOrderTotalCost()
            {
                // Arrange
                Init(nameof(CanCreateOrder_IncludingSubTotal_AndOrderTotal_AndOrderTotalCost));

                var workContextMock = new Mock<IWorkContext>();
                var userId = 123;
                workContextMock.Setup(x => x.GetCurrentUser()).ReturnsAsync(new User() { Id = userId });

                var orderRequest = new OrderFormVm()
                {
                    OrderStatus = OrderStatus.Pending,
                    CustomerId = 10,
                    ShippingAmount = 10000,
                    ShippingCost = 5000,
                    Discount = 2000,
                    TrackingNumber = "VAN",
                    OrderItems = new List<OrderItemVm> {
                        new OrderItemVm { ProductId = 1, Quantity = 5, ProductPrice = 11000 },
                        new OrderItemVm { ProductId = 2, Quantity = 3, ProductPrice = 15400 },
                    }
                };

                // Action
                long orderId;
                string error;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    var productRepo = new Repository<Product>(context);
                    var workContext = workContextMock.Object;
                    var orderService = new OrderService(orderRepo, productRepo, null, null, null, null, null, null, null, null, null, workContext);

                    (orderId, error) = await orderService.CreateOrderAsync(orderRequest);
                }

                Order createdOrder;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    createdOrder = await orderRepo.Query().FirstOrDefaultAsync(i => i.Id == orderId);
                }

                // Assert
                Assert.NotNull(createdOrder);
                Assert.Null(error);
                Assert.Equal(userId, createdOrder.CreatedById);
                Assert.Equal(orderRequest.OrderStatus, createdOrder.OrderStatus);
                Assert.Equal(orderRequest.TrackingNumber, createdOrder.TrackingNumber);

                var subTotal =orderRequest.OrderItems.Sum(item => item.SubTotal); 
                var orderTotalCost = orderRequest.OrderItems.Sum(item => item.SubTotalCost) + orderRequest.ShippingCost;
                Assert.Equal(subTotal, createdOrder.SubTotal);
                Assert.Equal(subTotal + orderRequest.ShippingAmount - orderRequest.Discount, createdOrder.OrderTotal);
                Assert.Equal(orderTotalCost, createdOrder.OrderTotalCost);
            }

            [Fact]
            public async Task ShouldFail_WhenShoppingCart_IsNull()
            {
                // Arrange
                var orderRequest = new OrderFormVm()
                {
                    OrderItems = null
                };

                // Action
                var orderService = new OrderService(null, null, null, null, null, null, null, null, null, null, null, null);
                var (orderId, error) = await orderService.CreateOrderAsync(orderRequest);

                // Assert
                Assert.Equal(0, orderId);
                Assert.NotNull(error);
            }

            [Fact]
            public async Task ShouldFail_WhenShoppingCart_IsEmpty()
            {
                // Arrange
                var orderRequest = new OrderFormVm()
                {
                    OrderItems = new List<OrderItemVm> {}
                };

                // Action
                var orderService = new OrderService(null, null, null, null, null, null, null, null, null, null, null, null);
                var (orderId, error) = await orderService.CreateOrderAsync(orderRequest);

                // Assert
                Assert.Equal(0, orderId);
                Assert.NotNull(error);
            }
        }

        public class UpdateOrderAsync
        {

            private DbContextOptions<SimplDbContext> _options;

            private long Init(string dbName)
            {
                _options = new DbContextOptionsBuilder<SimplDbContext>()
                    .UseInMemoryDatabase(databaseName: dbName)
                    .Options;

                long orderId;
                using (var context = new SimplDbContext(_options))
                {
                    var products = new List<Product>{
                        new Product() { Stock = 10 },
                        new Product() { Stock = 20 },
                    };
                    var productRepo = new Repository<Product>(context);
                    productRepo.AddRange(products);
                    productRepo.SaveChanges();

                    var order = new Order {
                        OrderItems = new List<OrderItem> {
                            new OrderItem { Product = products[0] },
                            new OrderItem { Product = products[1] }
                        }
                    };
                    var orderRepo = new Repository<Order>(context);
                    orderRepo.Add(order);
                    orderRepo.SaveChanges();

                    orderId = orderRepo.Query().First().Id;
                }

                return orderId;
            }

            [Fact]
            public async Task CanUpdateOrder_IncludingSubTotal_AndOrderTotal_AndOrderTotalCost()
            {
                // Arrange
                var orderId = Init(nameof(CanUpdateOrder_IncludingSubTotal_AndOrderTotal_AndOrderTotalCost));

                var orderRequest = new OrderFormVm()
                {
                    OrderId = orderId,
                    OrderStatus = OrderStatus.Paid,
                    CustomerId = 10,
                    ShippingAmount = 10000,
                    ShippingCost = 5000,
                    Discount = 2000,
                    TrackingNumber = "VAN",
                    OrderItems = new List<OrderItemVm> {
                        new OrderItemVm { ProductId = 1, Quantity = 5, ProductPrice = 11000 },
                        new OrderItemVm { ProductId = 2, Quantity = 3, ProductPrice = 15400 },
                    }
                };

                // Action
                string error;
                bool success;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    var productRepo = new Repository<Product>(context);
                    var orderService = new OrderService(orderRepo, productRepo, null, null, null, null, null, null, null, null, null, null);

                    (success, error) = await orderService.UpdateOrderAsync(orderRequest);
                }

                Order updatedOrder;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    updatedOrder = await orderRepo.Query().FirstOrDefaultAsync(i => i.Id == orderId);
                }

                // Assert
                Assert.True(success);
                Assert.Null(error);
                Assert.NotNull(updatedOrder);
                Assert.Equal(orderRequest.OrderStatus, updatedOrder.OrderStatus);
                Assert.Equal(orderRequest.TrackingNumber, updatedOrder.TrackingNumber);

                var subTotal =orderRequest.OrderItems.Sum(item => item.SubTotal); 
                var orderTotalCost = orderRequest.OrderItems.Sum(item => item.SubTotalCost) + orderRequest.ShippingCost;
                Assert.Equal(subTotal, updatedOrder.SubTotal);
                Assert.Equal(subTotal + orderRequest.ShippingAmount - orderRequest.Discount, updatedOrder.OrderTotal);
                Assert.Equal(orderTotalCost, updatedOrder.OrderTotalCost);
            }

            [Fact]
            public async Task ShouldFail_WhenShoppingCart_IsNull()
            {
                // Arrange
                var orderRequest = new OrderFormVm()
                {
                    OrderItems = null
                };

                // Action
                var orderService = new OrderService(null, null, null, null, null, null, null, null, null, null, null, null);
                var (success, error) = await orderService.UpdateOrderAsync(orderRequest);

                // Assert
                Assert.False(success);
                Assert.NotNull(error);
            }

            [Fact]
            public async Task ShouldFail_WhenShoppingCart_IsEmpty()
            {
                // Arrange
                var orderRequest = new OrderFormVm()
                {
                    OrderItems = new List<OrderItemVm> {}
                };

                // Action
                var orderService = new OrderService(null, null, null, null, null, null, null, null, null, null, null, null);
                var (success, error) = await orderService.UpdateOrderAsync(orderRequest);

                // Assert
                Assert.False(success);
                Assert.NotNull(error);
            }
        }
    }
}
