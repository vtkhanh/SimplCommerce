using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Web.SmartTable;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.Services;
using SimplCommerce.Module.Orders.ViewModels;
using SimplCommerce.Test.Shared.MockQueryable;
using Xunit;

namespace SimplCommerce.Module.Orders.Tests.Services
{
    public class SearchOrderServiceTests
    {
        private readonly Mock<IRepository<Order>> _mockOrderRepo;
        private readonly ISearchOrderService _service;

        public SearchOrderServiceTests()
        {
            var testOrders = CreateTestOrders().AsQueryable().BuildMock();
            _mockOrderRepo = new Mock<IRepository<Order>>();
            _mockOrderRepo.Setup(repo => repo.QueryAsNoTracking()).Returns(testOrders.Object);

            _service = new SearchOrderService(_mockOrderRepo.Object);
        }

        [Theory]
        [InlineData(OrderStatus.Pending)]
        [InlineData(OrderStatus.Processing)]
        [InlineData(OrderStatus.Complete)]
        public async Task GetOrdersAsync_CanSearchByStatusAsync(OrderStatus status)
        {
            // Arrange
            var search = new SearchOrderParametersVm { Status = status };

            // Action
            var orders = await _service.GetOrdersAsync(search, null);

            // Assert
            Assert.NotEmpty(orders);
            Assert.All(orders, order => order.Status.Equals(status));
        }

        [Theory]
        [InlineData("Mary")]
        [InlineData("Smith")]
        [InlineData("Henry")]
        [InlineData("Mal")]
        public async Task GetOrdersAsync_CanSearchByCustomerAsync(string customer)
        {
            // Arrange
            var search = new SearchOrderParametersVm { CustomerName = customer };

            // Action
            var orders = await _service.GetOrdersAsync(search, null);

            // Assert
            Assert.NotEmpty(orders);
            Assert.All(orders, order => order.CustomerName.Contains(customer));
        }

        [Theory]
        [InlineData("Dav")]
        [InlineData("Titan")]
        [InlineData("Z")]
        public async Task GetOrdersAsync_CanSearchByCreatedByAsync(string createdBy)
        {
            // Arrange
            var search = new SearchOrderParametersVm { CreatedBy = createdBy };

            // Action
            var orders = await _service.GetOrdersAsync(search, null);

            // Assert
            Assert.NotEmpty(orders);
            Assert.All(orders, order => order.CreatedBy.Contains(createdBy));
        }

        [Fact]
        public async Task GetOrdersAsync_CanSearchByCreatedOnAsync()
        {
            // Arrange
            const string DateFormat = "dd/MM/yyyy";

            var search = new SearchOrderParametersVm
            {
                CreatedOn = new PeriodVm
                {
                    After = new DateTime(2018, 12, 1),
                    Before = new DateTime(2019, 1, 1)
                }
            };

            // Action
            var orders = await _service.GetOrdersAsync(search, null);

            // Assert
            Assert.NotEmpty(orders);
            Assert.True(orders.All(order => DateTime.ParseExact(order.CreatedOn, DateFormat, CultureInfo.InvariantCulture) >= search.CreatedOn.After));
            Assert.True(orders.All(order => DateTime.ParseExact(order.CreatedOn, DateFormat, CultureInfo.InvariantCulture) <= search.CreatedOn.Before));
        }

        [Fact]
        public async Task GetOrdersAsync_CanSearchByCompletedOnAsync()
        {
            // Arrange
            const string DateFormat = "dd/MM/yyyy";

            var search = new SearchOrderParametersVm
            {
                CompletedOn = new PeriodVm
                {
                    After = new DateTime(2018, 12, 1),
                    Before = new DateTime(2019, 1, 1)
                }
            };

            // Action
            var orders = await _service.GetOrdersAsync(search, null);

            // Assert
            Assert.NotEmpty(orders);
            Assert.True(orders.All(order => DateTime.ParseExact(order.CompletedOn, DateFormat, CultureInfo.InvariantCulture) >= search.CompletedOn.After));
            Assert.True(orders.All(order => DateTime.ParseExact(order.CompletedOn, DateFormat, CultureInfo.InvariantCulture) <= search.CompletedOn.Before));
        }

        [Fact]
        public async Task GetOrdersAsync_ShouldSortIdDescendinglyByDefaultAsync()
        {
            // Arrange
            var search = new SearchOrderParametersVm();

            // Action
            var orders = await _service.GetOrdersAsync(search, null);

            // Assert
            Assert.NotEmpty(orders);
            Assert.True(orders.ElementAt(1).Id <= orders.ElementAt(0).Id);
            Assert.True(orders.ElementAt(2).Id <= orders.ElementAt(1).Id);
            Assert.True(orders.ElementAt(3).Id <= orders.ElementAt(2).Id);
        }

        [Fact]
        public async Task GetOrdersAsync_CanSortByCostAsync()
        {
            // Arrange
            var search = new SearchOrderParametersVm();
            var sort = new Sort { Predicate = "OrderTotalCost", Reverse = true };

            // Action
            var orders = await _service.GetOrdersAsync(search, sort);

            // Assert
            Assert.NotEmpty(orders);
            Assert.True(orders.ElementAt(1).Cost <= orders.ElementAt(0).Cost);
            Assert.True(orders.ElementAt(2).Cost <= orders.ElementAt(1).Cost);
            Assert.True(orders.ElementAt(3).Cost <= orders.ElementAt(2).Cost);
        }

        [Fact]
        public async Task GetOrdersAsync_CanSortByTotalAsync()
        {
            // Arrange
            var search = new SearchOrderParametersVm();
            var sort = new Sort { Predicate = "OrderTotal", Reverse = true };

            // Action
            var orders = await _service.GetOrdersAsync(search, sort);

            // Assert
            Assert.NotEmpty(orders);
            Assert.True(orders.ElementAt(1).Total <= orders.ElementAt(0).Total);
            Assert.True(orders.ElementAt(2).Total <= orders.ElementAt(1).Total);
            Assert.True(orders.ElementAt(3).Total <= orders.ElementAt(2).Total);
        }

        private List<Order> CreateTestOrders()
        {
            var orders = new List<Order>
            {
                new Order(1)
                {
                    OrderStatus = OrderStatus.Pending,
                    Customer = new User { FullName = "Mary" },
                    CreatedBy = new User { FullName = "David" },
                    TrackingNumber = "11111111",
                    CreatedOn = new DateTime(2018, 12, 30),
                    CompletedOn = new DateTime(2018, 12, 31),
                    OrderTotal = 100,
                    OrderTotalCost = 90
                },
                new Order(2)
                {
                    OrderStatus = OrderStatus.Processing,
                    Customer = new User { FullName = "Malice" },
                    CreatedBy = new User { FullName = "Titan" },
                    TrackingNumber = "22222222",
                    CreatedOn = new DateTime(2019, 1, 1),
                    CompletedOn = new DateTime(2019, 1, 30),
                    OrderTotal = 1000,
                    OrderTotalCost = 900
                },
                new Order(3)
                {
                    OrderStatus = OrderStatus.Pending,
                    Customer = new User { FullName = "John Smith" },
                    CreatedBy = new User { FullName = "Zeus" },
                    TrackingNumber = "33333333",
                    CreatedOn = new DateTime(2018, 12, 30),
                    CompletedOn = new DateTime(2019, 1, 30),
                    OrderTotal = 389,
                    OrderTotalCost = 350
                },
                new Order(4)
                {
                    OrderStatus = OrderStatus.Complete,
                    Customer = new User { FullName = "Henry" },
                    CreatedBy = new User { FullName = "Titan" },
                    TrackingNumber = "44444444",
                    CreatedOn = new DateTime(2019, 1, 20),
                    CompletedOn = new DateTime(2019, 2, 1),
                    OrderTotal = 999,
                    OrderTotalCost = 990
                },
            };
            return orders;
        }
    }
}
