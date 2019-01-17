using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using SimplCommerce.Infrastructure;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Catalog.Models;
using SimplCommerce.Module.Core.Data;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.Services;
using SimplCommerce.Module.Orders.ViewModels;
using SimplCommerce.Test.Shared.MockQueryable;
using Xunit;

namespace SimplCommerce.Module.Orders.Tests.Services
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
                        var products = new List<Product>{
                            new Product() { Stock = 10, Price = 11000, Cost = 10000 },
                            new Product() { Stock = 20, Price = 15400, Cost = 14000 },
                        };
                        order = new Order()
                        {
                            OrderStatus = OrderStatus.Pending,
                            CustomerId = 10,
                            ShippingAmount = 10000,
                            ShippingCost = 5000,
                            Discount = 2000,
                            OrderItems = new List<OrderItem> {
                                new OrderItem { Product = products[0], Quantity = 5, ProductPrice = products[0].Price },
                                new OrderItem { Product = products[1], Quantity = 3, ProductPrice = products[1].Price },
                            }
                        };
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
                Order order;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    order = await orderRepo.QueryAsNoTracking().FirstAsync();
                }

                // Action
                GetOrderVm updatedOrder;
                string error;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    var orderService = new OrderService(orderRepo, null, null, null, null, null, null, null, null, null, null);
                    (updatedOrder, error) = await orderService.UpdateStatusAsync(order.Id, status);
                }

                // Assert
                Assert.NotNull(updatedOrder);
                Assert.True(error.IsNullOrEmpty());
                Assert.Equal(status, updatedOrder.OrderStatus);
            }

            [Fact]
            public async Task WithCancelledStatus_ShouldResetQuantities()
            {
                // Arrange
                Order order;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    order = await orderRepo.QueryAsNoTracking()
                        .Include(item => item.OrderItems).ThenInclude(item => item.Product)
                        .FirstAsync();
                }

                // Action
                Order updatedOrder;
                string error;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    var orderService = new OrderService(orderRepo, null, null, null, null, null, null, null, null, null, null);
                    (_, error) = await orderService.UpdateStatusAsync(order.Id, OrderStatus.Cancelled);

                    updatedOrder = await orderRepo.QueryAsNoTracking()
                        .Include(item => item.OrderItems).ThenInclude(item => item.Product)
                        .FirstAsync(item => item.Id == order.Id);
                }

                // Assert
                Assert.NotNull(updatedOrder);
                Assert.True(error.IsNullOrEmpty());
                Assert.Equal(OrderStatus.Cancelled, updatedOrder.OrderStatus);

                Assert.Equal(0, updatedOrder.SubTotal);
                Assert.Equal(order.ShippingAmount, updatedOrder.OrderTotal);
                Assert.Equal(order.ShippingCost, updatedOrder.OrderTotalCost);
                Assert.Equal(order.OrderItems[0].Product.Stock + order.OrderItems[0].Quantity, updatedOrder.OrderItems[0].Product.Stock);
                Assert.Equal(order.OrderItems[1].Product.Stock + order.OrderItems[1].Quantity, updatedOrder.OrderItems[1].Product.Stock);
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
                GetOrderVm updatedOrder;
                string error;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    var orderService = new OrderService(orderRepo, null, null, null, null, null, null, null, null, null, null);
                    (updatedOrder, error) = await orderService.UpdateStatusAsync(order.Id + 1, OrderStatus.Complete);
                }

                // Assert
                Assert.Null(updatedOrder);
                Assert.True(error.HasValue());
            }

            [Fact]
            public async Task WithCompleteStatus_ShouldUpdateCompletedOn()
            {
                // Arrange
                Order order;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    order = await orderRepo.QueryAsNoTracking().FirstAsync();
                }

                // Action
                GetOrderVm updatedOrder;
                string error;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    var orderService = new OrderService(orderRepo, null, null, null, null, null, null, null, null, null, null);
                    (updatedOrder, error) = await orderService.UpdateStatusAsync(order.Id, OrderStatus.Complete);
                }

                // Assert
                Assert.NotNull(updatedOrder);
                Assert.True(updatedOrder.CompletedOn.HasValue);
            }

            [Theory]
            [InlineData(OrderStatus.Processing)]
            [InlineData(OrderStatus.Shipped)]
            [InlineData(OrderStatus.Paid)]
            [InlineData(OrderStatus.Cancelled)]
            public async Task WithBeingCompletedOverOneDay_ShouldReturnError(OrderStatus status)
            {
                // Arrange
                const long TestOrderId = 10;
                var mockOrderRepo = new Mock<IRepository<Order>>();
                var order = new Order(TestOrderId)
                {
                    CompletedOn = DateTimeOffset.Now.AddDays(-2),
                    OrderStatus = OrderStatus.Complete
                };
                var mockOrders = new Order[] { order }.AsQueryable().BuildMock();

                mockOrderRepo.Setup(repo => repo.Query()).Returns(mockOrders.Object);

                // Action
                var orderService = new OrderService(mockOrderRepo.Object, null, null, null, null, null, null, null, null, null, null);
                (GetOrderVm orderResult, string error) = await orderService.UpdateStatusAsync(TestOrderId, status);

                // Assert
                Assert.Null(orderResult);
                Assert.NotNull(error);
                Assert.NotEmpty(error);
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
                    var orderService = new OrderService(orderRepo, null, null, null, null, null, null, null, null, null, null);
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
                    var orderService = new OrderService(orderRepo, null, null, null, null, null, null, null, null, null, null);
                    (ok, error) = await orderService.UpdateTrackingNumberAsync(order.Id + 1, trackingNumber);
                }

                // Assert
                Assert.False(ok);
                Assert.True(error.HasValue());
            }

            [Fact]
            public async Task WithBeingCompletedOverOneDay_ShouldReturnError()
            {
                // Arrange
                const long TestOrderId = 10;
                const string TestTrackingNumber = "aTrackingNumber123";
                var mockOrderRepo = new Mock<IRepository<Order>>();
                var order = new Order(TestOrderId)
                {
                    CompletedOn = DateTimeOffset.Now.AddDays(-2),
                    OrderStatus = OrderStatus.Complete
                };
                var mockOrders = new Order[] { order }.AsQueryable().BuildMock();

                mockOrderRepo.Setup(repo => repo.Query()).Returns(mockOrders.Object);

                // Action
                var orderService = new OrderService(mockOrderRepo.Object, null, null, null, null, null, null, null, null, null, null);
                (bool success, string error) = await orderService.UpdateTrackingNumberAsync(TestOrderId, TestTrackingNumber);

                // Assert
                Assert.False(success);
                Assert.NotNull(error);
                Assert.NotEmpty(error);

            }
        }

        public class CreateOrderAsync
        {
            private DbContextOptions<SimplDbContext> _options;

            private IList<Product> Init(string dbName)
            {
                _options = new DbContextOptionsBuilder<SimplDbContext>()
                    .UseInMemoryDatabase(databaseName: dbName)
                    .Options;

                IList<Product> addedProducts;
                using (var context = new SimplDbContext(_options))
                {
                    var products = new List<Product>{
                        new Product() { Stock = 10, Price = 11000, Cost = 10000 },
                        new Product() { Stock = 20, Price = 15400, Cost = 14000 },
                    };
                    var productRepo = new Repository<Product>(context);
                    productRepo.AddRange(products);
                    productRepo.SaveChanges();

                    addedProducts = productRepo.Query().ToList();
                }

                return addedProducts;
            }

            [Fact]
            public async Task CanCreateOrder()
            {
                // Arrange
                var products = Init(nameof(CanCreateOrder));

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
                    PaymentProviderId = 1,
                    Note = "some note here",
                    OrderItems = new List<OrderItemVm> {
                        new OrderItemVm { ProductId = products[0].Id, Quantity = 5, ProductPrice = products[0].Price, ProductCost = products[0].Cost },
                        new OrderItemVm { ProductId = products[1].Id, Quantity = 3, ProductPrice = products[1].Price, ProductCost = products[1].Cost },
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
                    var orderService = new OrderService(orderRepo, productRepo, null, null, null, null, null, null, null, workContext, null);

                    (orderId, error) = await orderService.CreateOrderAsync(orderRequest);
                }

                Order createdOrder;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    createdOrder = await orderRepo.QueryAsNoTracking()
                        .Include(item => item.OrderItems).ThenInclude(item => item.Product)
                        .FirstOrDefaultAsync(i => i.Id == orderId);
                }

                // Assert
                Assert.NotNull(createdOrder);
                Assert.Null(error);
                Assert.Equal(userId, createdOrder.CreatedById);
                Assert.Equal(orderRequest.OrderStatus, createdOrder.OrderStatus);
                Assert.Equal(orderRequest.TrackingNumber, createdOrder.TrackingNumber);
                Assert.Equal(orderRequest.PaymentProviderId, createdOrder.PaymentProviderId);
                Assert.Equal(orderRequest.Note, createdOrder.Note);

                var subTotal = orderRequest.OrderItems.Sum(item => item.SubTotal);
                var orderTotalCost = orderRequest.OrderItems.Sum(item => item.SubTotalCost) + orderRequest.ShippingCost;
                Assert.Equal(subTotal, createdOrder.SubTotal);
                Assert.Equal(subTotal + orderRequest.ShippingAmount - orderRequest.Discount, createdOrder.OrderTotal);
                Assert.Equal(orderTotalCost, createdOrder.OrderTotalCost);
                Assert.Equal(products[0].Stock - orderRequest.OrderItems[0].Quantity, createdOrder.OrderItems[0].Product.Stock);
                Assert.Equal(products[1].Stock - orderRequest.OrderItems[1].Quantity, createdOrder.OrderItems[1].Product.Stock);
            }

            [Fact]
            public async Task WithCanceledStatus_ShouldResetOrderItemQuantities()
            {
                // Arrange
                var products = Init(nameof(WithCanceledStatus_ShouldResetOrderItemQuantities));

                var workContextMock = new Mock<IWorkContext>();
                var userId = 123;
                workContextMock.Setup(x => x.GetCurrentUser()).ReturnsAsync(new User() { Id = userId });

                var orderRequest = new OrderFormVm()
                {
                    OrderStatus = OrderStatus.Cancelled,
                    CustomerId = 10,
                    ShippingAmount = 10000,
                    ShippingCost = 5000,
                    Discount = 2000,
                    TrackingNumber = "VAN",
                    PaymentProviderId = 2,
                    OrderItems = new List<OrderItemVm> {
                        new OrderItemVm { ProductId = products[0].Id, Quantity = 5, ProductPrice = products[0].Price, ProductCost = products[0].Cost },
                        new OrderItemVm { ProductId = products[1].Id, Quantity = 3, ProductPrice = products[1].Price, ProductCost = products[1].Cost },
                    }
                };

                // Action
                string error;
                long orderId;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    var productRepo = new Repository<Product>(context);
                    var workContext = workContextMock.Object;
                    var orderService = new OrderService(orderRepo, productRepo, null, null, null, null, null, null, null, workContext, null);

                    (orderId, error) = await orderService.CreateOrderAsync(orderRequest);
                }

                Order createdOrder;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    createdOrder = await orderRepo.QueryAsNoTracking()
                        .Include(item => item.OrderItems).ThenInclude(item => item.Product)
                        .FirstOrDefaultAsync(i => i.Id == orderId);
                }

                // Assert
                Assert.Null(error);
                Assert.NotNull(createdOrder);
                Assert.Equal(orderRequest.OrderStatus, createdOrder.OrderStatus);
                Assert.Equal(orderRequest.TrackingNumber, createdOrder.TrackingNumber);
                Assert.Equal(orderRequest.PaymentProviderId, createdOrder.PaymentProviderId);

                Assert.Equal(0, createdOrder.SubTotal);
                Assert.Equal(orderRequest.ShippingAmount, createdOrder.OrderTotal);
                Assert.Equal(orderRequest.ShippingCost, createdOrder.OrderTotalCost);
                Assert.Equal(products[0].Stock, createdOrder.OrderItems[0].Product.Stock);
                Assert.Equal(products[1].Stock, createdOrder.OrderItems[1].Product.Stock);
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
                var orderService = new OrderService(null, null, null, null, null, null, null, null, null, null, null);
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
                    OrderItems = new List<OrderItemVm> { }
                };

                // Action
                var orderService = new OrderService(null, null, null, null, null, null, null, null, null, null, null);
                var (orderId, error) = await orderService.CreateOrderAsync(orderRequest);

                // Assert
                Assert.Equal(0, orderId);
                Assert.NotNull(error);
            }
        }

        public class UpdateOrderAsync
        {
            private DbContextOptions<SimplDbContext> _options;

            private (long, IList<Product>) Init(string dbName)
            {
                _options = new DbContextOptionsBuilder<SimplDbContext>()
                    .UseInMemoryDatabase(databaseName: dbName)
                    .Options;

                long orderId;
                IList<Product> addedProducts;
                using (var context = new SimplDbContext(_options))
                {
                    var products = new List<Product>{
                        new Product() { Stock = 10, Price = 11000, Cost = 10000 },
                        new Product() { Stock = 20, Price = 15400, Cost = 14000 },
                    };
                    var productRepo = new Repository<Product>(context);
                    productRepo.AddRange(products);
                    productRepo.SaveChanges();
                    addedProducts = productRepo.Query().ToList();

                    var order = new Order
                    {
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

                return (orderId, addedProducts);
            }

            [Fact]
            public async Task CanUpdateOrder()
            {
                // Arrange
                var (orderId, products) = Init(nameof(CanUpdateOrder));

                var orderRequest = new OrderFormVm()
                {
                    OrderId = orderId,
                    OrderStatus = OrderStatus.Paid,
                    CustomerId = 10,
                    ShippingAmount = 10000,
                    ShippingCost = 5000,
                    Discount = 2000,
                    TrackingNumber = "VAN",
                    PaymentProviderId = 3,
                    Note = "some note here",
                    OrderItems = new List<OrderItemVm> {
                        new OrderItemVm { ProductId = products[0].Id, Quantity = 5, ProductPrice = products[0].Price, ProductCost = products[0].Cost },
                        new OrderItemVm { ProductId = products[1].Id, Quantity = 3, ProductPrice = products[1].Price, ProductCost = products[1].Cost },
                    }
                };

                // Action
                string error;
                bool success;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    var productRepo = new Repository<Product>(context);
                    var orderService = new OrderService(orderRepo, productRepo, null, null, null, null, null, null, null, null, null);

                    (success, error) = await orderService.UpdateOrderAsync(orderRequest);
                }

                Order updatedOrder;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    updatedOrder = await orderRepo.QueryAsNoTracking()
                        .Include(item => item.OrderItems).ThenInclude(item => item.Product)
                        .FirstOrDefaultAsync(i => i.Id == orderId);
                }

                // Assert
                Assert.True(success);
                Assert.Null(error);
                Assert.NotNull(updatedOrder);
                Assert.Equal(orderRequest.OrderStatus, updatedOrder.OrderStatus);
                Assert.Equal(orderRequest.TrackingNumber, updatedOrder.TrackingNumber);
                Assert.Equal(orderRequest.PaymentProviderId, updatedOrder.PaymentProviderId);
                Assert.Equal(orderRequest.Note, updatedOrder.Note);

                var subTotal = orderRequest.OrderItems.Sum(item => item.SubTotal);
                var orderTotalCost = orderRequest.OrderItems.Sum(item => item.SubTotalCost) + orderRequest.ShippingCost;
                Assert.Equal(subTotal, updatedOrder.SubTotal);
                Assert.Equal(subTotal + orderRequest.ShippingAmount - orderRequest.Discount, updatedOrder.OrderTotal);
                Assert.Equal(orderTotalCost, updatedOrder.OrderTotalCost);
                Assert.Equal(products[0].Stock - orderRequest.OrderItems[0].Quantity, updatedOrder.OrderItems[0].Product.Stock);
                Assert.Equal(products[1].Stock - orderRequest.OrderItems[1].Quantity, updatedOrder.OrderItems[1].Product.Stock);
            }

            [Fact]
            public async Task WithCanceledStatus_ShouldResetOrderItemQuantities()
            {
                // Arrange
                var (orderId, products) = Init(nameof(WithCanceledStatus_ShouldResetOrderItemQuantities));

                var orderRequest = new OrderFormVm()
                {
                    OrderId = orderId,
                    OrderStatus = OrderStatus.Cancelled,
                    CustomerId = 10,
                    ShippingAmount = 10000,
                    ShippingCost = 5000,
                    Discount = 2000,
                    TrackingNumber = "VAN",
                    PaymentProviderId = 2,
                    OrderItems = new List<OrderItemVm> {
                        new OrderItemVm { ProductId = products[0].Id, Quantity = 5, ProductPrice = products[0].Price, ProductCost = products[0].Cost },
                        new OrderItemVm { ProductId = products[1].Id, Quantity = 3, ProductPrice = products[1].Price, ProductCost = products[1].Cost },
                    }
                };

                // Action
                string error;
                bool success;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    var productRepo = new Repository<Product>(context);
                    var orderService = new OrderService(orderRepo, productRepo, null, null, null, null, null, null, null, null, null);

                    (success, error) = await orderService.UpdateOrderAsync(orderRequest);
                }

                Order updatedOrder;
                using (var context = new SimplDbContext(_options))
                {
                    var orderRepo = new Repository<Order>(context);
                    updatedOrder = await orderRepo.QueryAsNoTracking()
                        .Include(item => item.OrderItems).ThenInclude(item => item.Product)
                        .FirstOrDefaultAsync(i => i.Id == orderId);
                }

                // Assert
                Assert.True(success);
                Assert.Null(error);
                Assert.NotNull(updatedOrder);
                Assert.Equal(orderRequest.OrderStatus, updatedOrder.OrderStatus);
                Assert.Equal(orderRequest.TrackingNumber, updatedOrder.TrackingNumber);
                Assert.Equal(orderRequest.PaymentProviderId, updatedOrder.PaymentProviderId);

                Assert.Equal(0, updatedOrder.SubTotal);
                Assert.Equal(orderRequest.ShippingAmount, updatedOrder.OrderTotal);
                Assert.Equal(orderRequest.ShippingCost, updatedOrder.OrderTotalCost);

                Assert.Equal(products[0].Stock, updatedOrder.OrderItems[0].Product.Stock);
                Assert.Equal(products[1].Stock, updatedOrder.OrderItems[1].Product.Stock);
            }

            [Fact]
            public async Task WhenShoppingCart_IsNull_ShouldFail()
            {
                // Arrange
                var orderRequest = new OrderFormVm()
                {
                    OrderItems = null
                };

                // Action
                var orderService = new OrderService(null, null, null, null, null, null, null, null, null, null, null);
                var (success, error) = await orderService.UpdateOrderAsync(orderRequest);

                // Assert
                Assert.False(success);
                Assert.NotNull(error);
            }

            [Fact]
            public async Task WhenShoppingCart_IsEmpty_ShouldFail()
            {
                // Arrange
                var orderRequest = new OrderFormVm()
                {
                    OrderItems = new List<OrderItemVm> { }
                };

                // Action
                var orderService = new OrderService(null, null, null, null, null, null, null, null, null, null, null);
                var (success, error) = await orderService.UpdateOrderAsync(orderRequest);

                // Assert
                Assert.False(success);
                Assert.NotNull(error);
            }

            [Fact]
            public async Task WhenOrder_IsCompletedOverOneDay_ShouldFail()
            {
                // Arrange
                const long TestOrderId = 10;
                var orderRequest = new OrderFormVm() { OrderId = TestOrderId };
                var mockOrderRepo = new Mock<IRepository<Order>>();
                var order = new Order(TestOrderId)
                {
                    CompletedOn = DateTimeOffset.Now.AddDays(-2),
                    OrderStatus = OrderStatus.Complete
                };
                var mockOrders = new Order[] { order }.AsQueryable().BuildMock();

                mockOrderRepo.Setup(repo => repo.Query()).Returns(mockOrders.Object);

                // Action
                var orderService = new OrderService(mockOrderRepo.Object, null, null, null, null, null, null, null, null, null, null);
                (bool success, string error) = await orderService.UpdateOrderAsync(orderRequest);

                // Assert
                Assert.False(success);
                Assert.NotNull(error);
                Assert.NotEmpty(error);
            }
        }

        public class GetOrderOwnerIdAsync
        {
            public GetOrderOwnerIdAsync()
            {
            }

            [Fact]
            public async Task CanReturnOwnerId()
            {
                // Arrange
                const long CreatedById = 10;
                long ownerId;
                long orderId;
                var options = new DbContextOptionsBuilder<SimplDbContext>()
                    .UseInMemoryDatabase(databaseName: nameof(CanReturnOwnerId))
                    .Options;

                using (var context = new SimplDbContext(options))
                {
                    var order = new Order { CreatedById = CreatedById };
                    var orderRepo = new Repository<Order>(context);
                    orderRepo.Add(order);
                    orderRepo.SaveChanges();

                    orderId = orderRepo.Query().First().Id;
                }

                // Action
                using (var context = new SimplDbContext(options))
                {
                    var orderRepo = new Repository<Order>(context);
                    var orderService = new OrderService(orderRepo, null, null, null, null, null, null, null, null, null, null);

                    ownerId = await orderService.GetOrderOwnerIdAsync(orderId);
                }

                // Assert
                Assert.Equal(CreatedById, ownerId);
            }

            [Fact]
            public async Task WithOrderNotFound_ShouldReturnDefault()
            {
                // Arrange
                const long CreatedById = 10;
                long ownerId;
                long orderId;
                var options = new DbContextOptionsBuilder<SimplDbContext>()
                    .UseInMemoryDatabase(databaseName: nameof(WithOrderNotFound_ShouldReturnDefault))
                    .Options;

                using (var context = new SimplDbContext(options))
                {
                    var order = new Order { CreatedById = CreatedById };
                    var orderRepo = new Repository<Order>(context);
                    orderRepo.Add(order);
                    orderRepo.SaveChanges();

                    orderId = orderRepo.Query().First().Id;
                }

                // Action
                using (var context = new SimplDbContext(options))
                {
                    var orderRepo = new Repository<Order>(context);
                    var orderService = new OrderService(orderRepo, null, null, null, null, null, null, null, null, null, null);

                    ownerId = await orderService.GetOrderOwnerIdAsync(orderId + 1);
                }

                // Assert
                Assert.Equal(default(long), ownerId);
            }
        }
    }
}