using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.Services;
using SimplCommerce.Test.Shared.MockQueryable;
using Xunit;

namespace SimplCommerce.Module.Orders.Tests.Services
{
    public class ReportServiceTest
    {
        [Fact]
        public async Task GetRevenueReportAsync_ShouldReturnResult_ForAllSellers()
        {
            // Arrange
            var testOrders = CreateTestOrders();
            var mockOrders = testOrders.AsQueryable().BuildMock();
            var mockRepo = new Mock<IRepository<Order>>();
            mockRepo.Setup(repo => repo.QueryAsNoTracking()).Returns(mockOrders.Object);

            // Action
            var reportService = new ReportService(mockRepo.Object);
            var report = await reportService.GetRevenueReportAsync(DateTime.Now, null);

            // Assert
            Assert.NotNull(report);
            var subTotal = testOrders.Sum(order => order.SubTotal);
            var total = testOrders.Sum(order => order.OrderTotal);
            var cost = testOrders.Sum(order => order.OrderTotalCost);
            var profit = total - cost;
            Assert.Equal(subTotal, report.SubTotals[0]);
            Assert.Equal(total, report.Totals[0]);
            Assert.Equal(cost, report.Costs[0]);
            Assert.Equal(profit, report.Profits[0]);
        }

        [Fact]
        public async Task GetRevenueReportAsync_ShouldReturnResult_ForOneSeller()
        {
            // Arrange
            var testOrders = CreateTestOrders();
            var sellerId = testOrders[1].CreatedById;
            var mockOrders = testOrders.AsQueryable().BuildMock();
            var mockRepo = new Mock<IRepository<Order>>();
            mockRepo.Setup(repo => repo.QueryAsNoTracking()).Returns(mockOrders.Object);

            // Action
            var reportService = new ReportService(mockRepo.Object);
            var report = await reportService.GetRevenueReportAsync(DateTime.Now, sellerId);

            // Assert
            Assert.NotNull(report);
            var subTotal = testOrders.Where(order => order.CreatedById == sellerId).Sum(order => order.SubTotal);
            var total = testOrders.Where(order => order.CreatedById == sellerId).Sum(order => order.OrderTotal);
            var cost = testOrders.Where(order => order.CreatedById == sellerId).Sum(order => order.OrderTotalCost);
            var profit = total - cost;
            Assert.Equal(subTotal, report.SubTotals[0]);
            Assert.Equal(total, report.Totals[0]);
            Assert.Equal(cost, report.Costs[0]);
            Assert.Equal(profit, report.Profits[0]);
        }

        [Fact]
        public async Task GetRevenueReportBySellerAsync_ShouldReturnResult()
        {
            // Arrange
            var testOrders = CreateTestOrders();
            var sellerId = testOrders[1].CreatedById;
            var mockOrders = testOrders.AsQueryable().BuildMock();
            var mockRepo = new Mock<IRepository<Order>>();
            mockRepo.Setup(repo => repo.QueryAsNoTracking()).Returns(mockOrders.Object);

            // Action
            var reportService = new ReportService(mockRepo.Object);
            var report = await reportService.GetRevenueReportBySellerAsync(DateTime.Now, sellerId);

            // Assert
            Assert.NotNull(report);
            var subTotal = testOrders.Where(order => order.CreatedById == sellerId).Sum(order => order.SubTotal);
            Assert.Equal(subTotal, report.SubTotals[0]);
            Assert.Empty(report.Totals);
            Assert.Empty(report.Costs);
            Assert.Empty(report.Profits);

        }

        private IList<Order> CreateTestOrders()
        {
            var order1 = new Order
            {
                CreatedById = 1,
                SubTotal = 20,
                OrderTotal = 18,
                OrderTotalCost = 15,
                CreatedOn = DateTimeOffset.Now,
                OrderStatus = OrderStatus.Complete
            };
            var order2 = new Order
            {
                CreatedById = 2,
                SubTotal = 30,
                OrderTotal = 28,
                OrderTotalCost = 20,
                CreatedOn = DateTimeOffset.Now,
                OrderStatus = OrderStatus.Complete
            };
            var order3 = new Order
            {
                CreatedById = 2,
                SubTotal = 40,
                OrderTotal = 35,
                OrderTotalCost = 30,
                CreatedOn = DateTimeOffset.Now,
                OrderStatus = OrderStatus.Complete
            };
            return new List<Order> { order1, order2, order3 };
        }
    }
}
