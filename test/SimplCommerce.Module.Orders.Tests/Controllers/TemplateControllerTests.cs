using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SimplCommerce.Infrastructure.ResultTypes;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Extensions.Constants;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Orders.Controllers;
using SimplCommerce.Module.Orders.Services;
using SimplCommerce.Module.Orders.ViewModels;
using Xunit;

namespace SimplCommerce.Module.Orders.Tests.Controllers
{
    public class TemplateControllerTests
    {
        private const string OrderListView = "OrderList";
        private const string OrderListSellerView = "OrderListSeller";
        private const string OrderFormView = "OrderForm";
        private const string OrderFormSellerView = "OrderFormSeller";
        private const string OrderFormRestrictedView = "OrderFormRestricted";

        private readonly TemplateController _controller;
        private readonly Mock<IWorkContext> _mockWorkContext;
        private readonly Mock<IOrderService> _mockOrderService;

        public TemplateControllerTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _mockWorkContext = new Mock<IWorkContext>();
            _controller = new TemplateController(_mockWorkContext.Object, _mockOrderService.Object);
        }

        [Fact]
        public void GetOrderList_WithSellerUser_ShouldReturnView()
        {
            // Arrange
            var mockUser = new Mock<ClaimsPrincipal>();
            mockUser.Setup(user => user.IsInRole(RoleName.Admin)).Returns(false);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = mockUser.Object }
            };

            // Action
            var result = _controller.GetOrderList();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(OrderListSellerView, viewResult.ViewName);
        }

        [Fact]
        public void GetOrderList_WithAdminUser_ShouldReturnView()
        {
            // Arrange
            var mockUser = new Mock<ClaimsPrincipal>();
            mockUser.Setup(user => user.IsInRole(RoleName.Admin)).Returns(true);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = mockUser.Object }
            };

            // Action
            var result = _controller.GetOrderList();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(OrderListView, viewResult.ViewName);
        }
        
        [Fact]
        public void GetOrderFormCreate_WithSellerUser_ShouldReturnView()
        {
            // Arrange
            var mockUser = new Mock<ClaimsPrincipal>();
            mockUser.Setup(user => user.IsInRole(RoleName.Admin)).Returns(false);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = mockUser.Object }
            };

            // Action
            var result = _controller.GetOrderFormCreate();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(OrderFormSellerView, viewResult.ViewName);
        }

        [Fact]
        public void GetOrderFormCreate_WithAdminUser_ShouldReturnView()
        {
            // Arrange
            var mockUser = new Mock<ClaimsPrincipal>();
            mockUser.Setup(user => user.IsInRole(RoleName.Admin)).Returns(true);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = mockUser.Object }
            };

            // Action
            var result = _controller.GetOrderFormCreate();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(OrderFormView, viewResult.ViewName);
        }

        [Theory]
        [InlineData(10, 10, true, OrderFormSellerView)] // Is owner, can edit
        [InlineData(10, 10, false, OrderFormRestrictedView)] // Is owner, cannot edit
        [InlineData(9, 10, true, OrderFormRestrictedView)] // Is not owner, can edit
        [InlineData(9, 10, false, OrderFormRestrictedView)] // Is not owner, cannot edit
        public async void GetOrderFormEdit_WithSellerUser_ShouldReturnView(long userId, long ownerId, 
            bool canEditOrder, string expectedView)
        {
            // Arrange
            const int OrderId = 1;
            var mockUser = new Mock<ClaimsPrincipal>();
            mockUser.Setup(user => user.IsInRole(RoleName.Admin)).Returns(false);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = mockUser.Object }
            };
            _mockWorkContext.Setup(context => context.GetCurrentUser()).Returns(Task.FromResult(new User { Id = userId }));

            var order = new GetOrderVm()
            {
                CanEdit = canEditOrder,
                CreatedById = ownerId
            };
            var orderResult = ActionFeedback<GetOrderVm>.Succeed(order);
            _mockOrderService.Setup(service => service.GetOrderAsync(OrderId)).Returns(Task.FromResult(orderResult));

            // Action
            var result = await _controller.GetOrderFormEdit(OrderId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(expectedView, viewResult.ViewName);
        }

        [Fact]
        public async Task GetOrderFormEdit_WithAdminUser_ShouldReturnView()
        {
            // Arrange
            const int OrderId = 1;
            var mockUser = new Mock<ClaimsPrincipal>();
            mockUser.Setup(user => user.IsInRole(RoleName.Admin)).Returns(true);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = mockUser.Object }
            };

            // Action
            var result = await _controller.GetOrderFormEdit(OrderId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(OrderFormView, viewResult.ViewName);
        }
    }
}
