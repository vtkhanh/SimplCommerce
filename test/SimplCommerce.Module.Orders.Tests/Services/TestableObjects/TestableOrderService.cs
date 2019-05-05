using Microsoft.AspNetCore.Http;
using Moq;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Catalog.Models;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Core.Services;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.Services;
using SimplCommerce.Module.Payments.Services;
using SimplCommerce.Module.Pricing.Services;
using SimplCommerce.Module.ShippingPrices.Services;
using SimplCommerce.Module.ShoppingCart.Models;
using SimplCommerce.Module.Tax.Services;

namespace SimplCommerce.Module.Orders.Tests.Services.TestableObjects
{
    public class TestableOrderService : OrderService
    {
        public Mock<IRepository<Order>> MockOrderRepo;
        public Mock<IRepository<Product>> MockProductRepo;
        public Mock<IRepository<Cart>> MockCartRepo;
        public Mock<IRepository<CartItem>> MockCartItemRepo;
        public Mock<IRepository<UserAddress>> MockUserAddressRepo;
        public Mock<ICouponService> MockCouponService;
        public Mock<ITaxService> MockTaxService;
        public Mock<IShippingPriceService> MockShippingPriceService;
        public Mock<IMediaService> MockMediaService;
        public Mock<IPaymentProviderService> MockPaymentProviderService;
        public Mock<IWorkContext> MockWorkContext;
        public Mock<IHttpContextAccessor> MockHttpContextAccessor;

        private TestableOrderService(
                Mock<IRepository<Order>> mockOrderRepo,
                Mock<IRepository<Product>> mockProductRepo,
                Mock<IRepository<Cart>> mockCartRepo,
                Mock<IRepository<CartItem>> mockCartItemRepo,
                Mock<IRepository<UserAddress>> mockUserAddressRepo,
                Mock<ICouponService> mockCouponService,
                Mock<ITaxService> mockTaxService,
                Mock<IShippingPriceService> mockShippingPriceService,
                Mock<IMediaService> mockMediaService,
                Mock<IPaymentProviderService> mockPaymentProviderService,
                Mock<IWorkContext> mockWorkContext,
                Mock<IHttpContextAccessor> mockHttpContextAccessor
            )
            : base(
                  mockOrderRepo.Object,
                  mockProductRepo.Object,
                  mockCartRepo.Object,
                  mockCartItemRepo.Object,
                  mockUserAddressRepo.Object,
                  mockCouponService.Object,
                  mockTaxService.Object,
                  mockShippingPriceService.Object,
                  mockMediaService.Object,
                  mockPaymentProviderService.Object,
                  mockWorkContext.Object,
                  mockHttpContextAccessor.Object
            )
        {
            MockOrderRepo = mockOrderRepo;
            MockProductRepo = mockProductRepo;
            MockCartRepo = mockCartRepo;
            MockCartItemRepo = mockCartItemRepo;
            MockUserAddressRepo = mockUserAddressRepo;
            MockCouponService = mockCouponService;
            MockTaxService = mockTaxService;
            MockShippingPriceService = mockShippingPriceService;
            MockMediaService = mockMediaService;
            MockPaymentProviderService = mockPaymentProviderService;
            MockWorkContext = mockWorkContext;
            MockHttpContextAccessor = mockHttpContextAccessor;
        }

        public static TestableOrderService Create() => 
            new TestableOrderService(
                    new Mock<IRepository<Order>>(),
                    new Mock<IRepository<Product>>(),
                    new Mock<IRepository<Cart>>(),
                    new Mock<IRepository<CartItem>>(),
                    new Mock<IRepository<UserAddress>>(),
                    new Mock<ICouponService>(),
                    new Mock<ITaxService>(),
                    new Mock<IShippingPriceService>(),
                    new Mock<IMediaService>(),
                    new Mock<IPaymentProviderService>(),
                    new Mock<IWorkContext>(),
                    new Mock<IHttpContextAccessor>()
                );
    }
}
