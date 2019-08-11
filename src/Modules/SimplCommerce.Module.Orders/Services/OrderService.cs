using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SimplCommerce.Infrastructure;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.ResultTypes;
using SimplCommerce.Module.Catalog.Models;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Extensions.Constants;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Core.Services;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.ViewModels;
using SimplCommerce.Module.Payments.Services;
using SimplCommerce.Module.Pricing.Services;
using SimplCommerce.Module.ShippingPrices.Services;
using SimplCommerce.Module.ShoppingCart.Models;
using SimplCommerce.Module.Tax.Services;

namespace SimplCommerce.Module.Orders.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<CartItem> _cartItemRepository;
        private readonly IRepository<UserAddress> _userAddressRepository;
        private readonly ICouponService _couponService;
        private readonly ITaxService _taxService;
        private readonly IShippingPriceService _shippingPriceService;
        private readonly IWorkContext _workContext;
        private readonly IMediaService _mediaService;
        private readonly IPaymentProviderService _paymentProviderService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(
            IRepository<Order> orderRepository,
            IRepository<Product> productRepo,
            IRepository<Cart> cartRepository,
            IRepository<CartItem> cartItemRepository,
            IRepository<UserAddress> userAddressRepository,
            ICouponService couponService,
            ITaxService taxService,
            IShippingPriceService shippingPriceService,
            IMediaService mediaService,
            IPaymentProviderService paymentProviderService,
            IWorkContext workContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _orderRepository = orderRepository;
            _productRepo = productRepo;
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _userAddressRepository = userAddressRepository;
            _couponService = couponService;
            _taxService = taxService;
            _shippingPriceService = shippingPriceService;
            _mediaService = mediaService;
            _paymentProviderService = paymentProviderService;
            _workContext = workContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ActionFeedback<GetOrderVm>> GetOrderAsync(long orderId)
        {
            var order = await _orderRepository.QueryAsNoTracking()
                .Include(item => item.OrderItems).ThenInclude(item => item.Product).ThenInclude(item => item.ThumbnailImage)
                .FirstOrDefaultAsync(item => item.Id == orderId);

            if (order == null)
                return ActionFeedback<GetOrderVm>.Fail($"Cannot find order with id {orderId}");

            var user = await _workContext.GetCurrentUser();
            var result = new GetOrderVm
            {
                OrderStatus = order.OrderStatus,
                CustomerId = order.CustomerId,
                SubTotal = order.SubTotal,
                SubTotalCost = order.OrderItems.Sum(item => item.Quantity * item.Product.Cost),
                ShippingAmount = order.ShippingAmount,
                ShippingCost = order.ShippingCost,
                Discount = order.Discount,
                OrderTotal = order.OrderTotal,
                OrderTotalCost = order.OrderTotalCost,
                TrackingNumber = order.TrackingNumber,
                CreatedById = order.CreatedById,
                VendorId = order.VendorId,
                Note = order.Note,
                IsShopeeOrder = order.IsShopeeOrder,
                OrderItems = order.OrderItems
                    .Select(item => new OrderItemVm
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        ProductName = item.Product.Name,
                        ProductSku = item.Product.Sku,
                        ProductCost = item.Product.Cost,
                        OriginalPrice = item.Product.Price,
                        ProductPrice = item.ProductPrice,
                        Stock = item.Product.Stock,
                        ProductImage = _mediaService.GetThumbnailUrl(item.Product.ThumbnailImage),
                        Quantity = item.Quantity
                    })
                    .OrderBy(item => item.Id).ToList(),
                PaymentProviderId = order.PaymentProviderId,
                PaymentProviderList = (await _paymentProviderService.GetListAsync(true))
                    .Select(item => new SelectListItem
                    {
                        Value = item.Id.ToString(),
                        Text = item.Description
                    }).ToList(),
                CanEdit = CanEditOrder(order)
            };

            return ActionFeedback<GetOrderVm>.Succeed(result);
        }

        public async Task<ActionFeedback<long>> CreateOrderAsync(OrderFormVm orderRequest)
        {
            if (orderRequest.OrderItems == null || !orderRequest.OrderItems.Any())
                return ActionFeedback<long>.Fail("Shopping cart cannot be empty");

            var user = await _workContext.GetCurrentUser();
            var order = new Order() { CreatedById = user.Id };

            var feedback = await UpdateOrderDetailsAsync(order, orderRequest);
            if (!feedback.Success)
                return ActionFeedback<long>.Fail(feedback.ErrorMessage);

            _orderRepository.Add(order);
            await _orderRepository.SaveChangesAsync();

            return ActionFeedback<long>.Succeed(order.Id);
        }

        public async Task<ActionFeedback> UpdateOrderAsync(OrderFormVm orderRequest)
        {
            if (orderRequest.OrderItems == null || !orderRequest.OrderItems.Any())
                return ActionFeedback.Fail("Shopping cart cannot be empty");

            var order = await _orderRepository.Query()
                .Include(item => item.OrderItems).ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(item => item.Id == orderRequest.OrderId);

            if (order == null)
                return ActionFeedback.Fail($"Cannot find order with id {orderRequest.OrderId}");
            if (!CanEditOrder(order))
                return ActionFeedback.Fail($"Order has been completed already!");

            var feedback = await UpdateOrderDetailsAsync(order, orderRequest);
            if (!feedback.Success)
                return feedback;

            await _orderRepository.SaveChangesAsync();

            return ActionFeedback.Succeed();
        }

        public async Task<Order> CreateOrder(User user, string paymentMethod)
        {
            var cart = await _cartRepository
               .Query()
               .Where(x => x.UserId == user.Id && x.IsActive).FirstOrDefaultAsync();

            if (cart == null)
                throw new ApplicationException($"Cart of user {user.Id} cannot be found");

            var shippingData = JsonConvert.DeserializeObject<DeliveryInformationVm>(cart.ShippingData);
            Address billingAddress;
            Address shippingAddress;
            if (shippingData.ShippingAddressId == 0)
            {
                var address = new Address
                {
                    AddressLine1 = shippingData.NewAddressForm.AddressLine1,
                    AddressLine2 = shippingData.NewAddressForm.AddressLine2,
                    ContactName = shippingData.NewAddressForm.ContactName,
                    CountryId = shippingData.NewAddressForm.CountryId,
                    StateOrProvinceId = shippingData.NewAddressForm.StateOrProvinceId,
                    DistrictId = shippingData.NewAddressForm.DistrictId,
                    City = shippingData.NewAddressForm.City,
                    PostalCode = shippingData.NewAddressForm.PostalCode,
                    Phone = shippingData.NewAddressForm.Phone
                };

                var userAddress = new UserAddress
                {
                    Address = address,
                    AddressType = AddressType.Shipping,
                    UserId = user.Id
                };

                _userAddressRepository.Add(userAddress);

                billingAddress = shippingAddress = address;
            }
            else
            {
                billingAddress = shippingAddress = _userAddressRepository.Query().Where(x => x.Id == shippingData.ShippingAddressId).Select(x => x.Address).First();
            }

            return await CreateOrder(user, paymentMethod, shippingData.ShippingMethod, billingAddress, shippingAddress);
        }

        public async Task<Order> CreateOrder(User user, string paymentMethod, string shippingMethodName, Address billingAddress, Address shippingAddress)
        {
            var cart = _cartRepository
                .Query()
                .Include(c => c.Items).ThenInclude(x => x.Product)
                .Where(x => x.UserId == user.Id && x.IsActive).FirstOrDefault();

            if (cart == null)
                throw new ApplicationException($"Cart of user {user.Id} cannot be found");

            var discount = await ApplyDiscount(user, cart);
            var shippingMethod = await ValidateShippingMethod(shippingMethodName, shippingAddress, cart);

            var orderBillingAddress = new OrderAddress()
            {
                AddressLine1 = billingAddress.AddressLine1,
                AddressLine2 = billingAddress.AddressLine2,
                ContactName = billingAddress.ContactName,
                CountryId = billingAddress.CountryId,
                StateOrProvinceId = billingAddress.StateOrProvinceId,
                DistrictId = billingAddress.DistrictId,
                City = billingAddress.City,
                PostalCode = billingAddress.PostalCode,
                Phone = billingAddress.Phone
            };

            var orderShippingAddress = new OrderAddress()
            {
                AddressLine1 = shippingAddress.AddressLine1,
                AddressLine2 = shippingAddress.AddressLine2,
                ContactName = shippingAddress.ContactName,
                CountryId = shippingAddress.CountryId,
                StateOrProvinceId = shippingAddress.StateOrProvinceId,
                DistrictId = shippingAddress.DistrictId,
                City = shippingAddress.City,
                PostalCode = shippingAddress.PostalCode,
                Phone = shippingAddress.Phone
            };

            var order = new Order
            {
                CreatedOn = DateTimeOffset.Now,
                CreatedById = user.Id,
                BillingAddress = orderBillingAddress,
                ShippingAddress = orderShippingAddress,
                PaymentMethod = paymentMethod,
            };

            foreach (var cartItem in cart.Items)
            {
                var taxPercent =
                    await _taxService.GetTaxPercent(cartItem.Product.TaxClassId, (long)shippingAddress.CountryId, (long)shippingAddress.StateOrProvinceId);
                var orderItem = new OrderItem
                {
                    Product = cartItem.Product,
                    ProductPrice = cartItem.Product.Price,
                    Quantity = cartItem.Quantity,
                    TaxPercent = taxPercent,
                    TaxAmount = cartItem.Quantity * (cartItem.Product.Price * taxPercent / 100)
                };
                order.AddOrderItem(orderItem);
            }

            order.CouponCode = cart.CouponCode;
            order.CouponRuleName = cart.CouponRuleName;
            order.Discount = discount;
            order.ShippingAmount = shippingMethod.Price;
            order.ShippingMethod = shippingMethod.Name;
            order.TaxAmount = order.OrderItems.Sum(x => x.TaxAmount);
            order.SubTotal = order.OrderItems.Sum(x => x.ProductPrice * x.Quantity);
            order.OrderTotal = order.SubTotal + order.TaxAmount + order.ShippingAmount - order.Discount;
            _orderRepository.Add(order);

            cart.IsActive = false;

            var vendorIds = cart.Items.Where(x => x.Product.VendorId.HasValue).Select(x => x.Product.VendorId.Value).Distinct();
            foreach (var vendorId in vendorIds)
            {
                var subOrder = new Order
                {
                    CreatedOn = DateTimeOffset.Now,
                    CreatedById = user.Id,
                    BillingAddress = orderBillingAddress,
                    ShippingAddress = orderShippingAddress,
                    VendorId = vendorId,
                    Parent = order
                };

                foreach (var cartItem in cart.Items.Where(x => x.Product.VendorId == vendorId))
                {
                    var taxPercent =
                        await _taxService.GetTaxPercent(cartItem.Product.TaxClassId, (long)shippingAddress.CountryId, (long)shippingAddress.StateOrProvinceId);
                    var orderItem = new OrderItem
                    {
                        Product = cartItem.Product,
                        ProductPrice = cartItem.Product.Price,
                        Quantity = cartItem.Quantity,
                        TaxPercent = taxPercent,
                        TaxAmount = cartItem.Quantity * (cartItem.Product.Price * taxPercent / 100)
                    };

                    subOrder.AddOrderItem(orderItem);
                }

                subOrder.SubTotal = subOrder.OrderItems.Sum(x => x.ProductPrice * x.Quantity);
                subOrder.TaxAmount = subOrder.OrderItems.Sum(x => x.TaxAmount);
                subOrder.OrderTotal = subOrder.SubTotal + subOrder.TaxAmount + subOrder.ShippingAmount - subOrder.Discount;
                _orderRepository.Add(subOrder);
            }

            _orderRepository.SaveChanges();

            return order;
        }

        public async Task<decimal> GetTax(long cartOwnerUserId, long countryId, long stateOrProvinceId)
        {
            decimal taxAmount = 0;
            var cart = await _cartRepository.Query().FirstOrDefaultAsync(x => x.UserId == cartOwnerUserId && x.IsActive);

            if (cart == null)
                throw new ApplicationException($"No active cart of user {cartOwnerUserId}");

            var cartItems = _cartItemRepository.Query()
                .Where(x => x.CartId == cart.Id)
                .Select(x => new CartItemVm
                {
                    Quantity = x.Quantity,
                    Price = x.Product.Price,
                    TaxClassId = x.Product.TaxClass.Id
                }).ToList();

            foreach (var cartItem in cartItems)
            {
                if (cartItem.TaxClassId.HasValue)
                {
                    var taxRate = await _taxService.GetTaxPercent(cartItem.TaxClassId, countryId, stateOrProvinceId);
                    taxAmount = taxAmount + cartItem.Quantity * cartItem.Price * taxRate / 100;
                }
            }

            return taxAmount;
        }

        public async Task<ActionFeedback> UpdateTrackingNumberAsync(long orderId, string trackingNumber)
        {
            var order = await _orderRepository.Query().FirstOrDefaultAsync(x => x.Id == orderId);

            if (order == null)
                return ActionFeedback.Fail($"Cannot find order with Id: {orderId}");
            if (!CanEditOrder(order))
                return ActionFeedback.Fail($"Order has been completed already!");

            var feedback = UpdateTrackingNumber(order, trackingNumber);
            if (!feedback.Success)
                return feedback;

            await _orderRepository.SaveChangesAsync();

            return ActionFeedback.Succeed();
        }

        public async Task<(GetOrderVm, string)> UpdateStatusAsync(long orderId, OrderStatus status)
        {
            var order = await _orderRepository.Query()
                .Include(item => item.OrderItems).ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(item => item.Id == orderId);

            if (order == null)
                return (null, $"Cannot find order with Id: {orderId}");
            if (!CanEditOrder(order))
                return (null, $"Order has been completed already!");

            UpdateStatusAndOrderTotal(order, status);

            await _orderRepository.SaveChangesAsync();

            var result = new GetOrderVm
            {
                OrderId = order.Id,
                SubTotal = order.SubTotal,
                OrderTotal = order.OrderTotal,
                OrderTotalCost = order.OrderTotalCost,
                OrderStatus = order.OrderStatus,
                CompletedOn = order.CompletedOn
            };

            return (result, null);
        }

        public async Task<(IList<GetOrderVm>, string)> UpdateStatusesAsync(IList<long> orderIds, OrderStatus status)
        {
            var orders = await _orderRepository.Query()
                .Include(item => item.OrderItems).ThenInclude(item => item.Product)
                .Where(item => orderIds.Contains(item.Id))
                .ToListAsync();
            var result = new List<GetOrderVm>();

            foreach (var order in orders)
            {
                if (!CanEditOrder(order))
                    continue;

                UpdateStatusAndOrderTotal(order, status);

                await _orderRepository.SaveChangesAsync();

                var orderVm = new GetOrderVm
                {
                    OrderId = order.Id,
                    SubTotal = order.SubTotal,
                    OrderTotal = order.OrderTotal,
                    OrderTotalCost = order.OrderTotalCost,
                    OrderStatus = order.OrderStatus,
                    CompletedOn = order.CompletedOn
                };
                result.Add(orderVm);
            }

            return (result, null);
        }

        public async Task<ActionFeedback> UpdateOrderStateAsync(OrderFormVm orderRequest)
        {
            var order = await _orderRepository.Query()
                .Include(item => item.OrderItems).ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(item => item.Id == orderRequest.OrderId);

            if (order == null)
                return ActionFeedback.Fail($"Cannot find order with Id: {orderRequest.OrderId}");
            if (!CanEditOrder(order))
                return ActionFeedback.Fail($"Order has been completed already!");

            order.TrackingNumber = orderRequest.TrackingNumber;
            order.PaymentProviderId = orderRequest.PaymentProviderId;
            UpdateStatusAndOrderTotal(order, orderRequest.OrderStatus);

            await _orderRepository.SaveChangesAsync();

            return ActionFeedback.Succeed();
        }

        public async Task<long> GetOrderOwnerIdAsync(long orderId)
        {
            var order = await _orderRepository.Query().FirstOrDefaultAsync(x => x.Id == orderId);

            if (order == null)
                return default;

            return order.CreatedById;
        }

        private async Task<decimal> ApplyDiscount(User user, Cart cart)
        {
            decimal discount = 0;
            if (!string.IsNullOrWhiteSpace(cart.CouponCode))
            {
                var cartInfoForCoupon = new CartInfoForCoupon
                {
                    Items = cart.Items.Select(x => new CartItemForCoupon { ProductId = x.ProductId, Quantity = x.Quantity }).ToList()
                };
                var couponValidationResult = await _couponService.Validate(cart.CouponCode, cartInfoForCoupon);
                if (couponValidationResult.Succeeded)
                {
                    discount = couponValidationResult.DiscountAmount;
                    _couponService.AddCouponUsage(user.Id, couponValidationResult.CouponId);
                }
                else
                {
                    throw new ApplicationException($"Unable to apply coupon {cart.CouponCode}. {couponValidationResult.ErrorMessage}");
                }
            }

            return discount;
        }

        private async Task<ShippingPrice> ValidateShippingMethod(string shippingMethodName, Address shippingAddress, Cart cart)
        {
            var applicableShippingPrices = await _shippingPriceService.GetApplicableShippingPrices(new GetShippingPriceRequest
            {
                OrderAmount = cart.Items.Sum(x => x.Product.Price * x.Quantity),
                ShippingAddress = shippingAddress
            });

            var shippingMethod = applicableShippingPrices.FirstOrDefault(x => x.Name == shippingMethodName);
            if (shippingMethod == null)
                throw new ApplicationException($"Invalid shipping method {shippingMethod}");

            return shippingMethod;
        }

        private async Task<ActionFeedback> UpdateOrderDetailsAsync(Order order, OrderFormVm orderRequest)
        {
            order.CustomerId = orderRequest.CustomerId;
            order.ShippingAmount = orderRequest.ShippingAmount;
            order.ShippingCost = orderRequest.ShippingCost;
            order.Discount = orderRequest.Discount;
            order.PaymentProviderId = orderRequest.PaymentProviderId;
            order.Note = orderRequest.Note;
            order.IsShopeeOrder = orderRequest.IsShopeeOrder;

            var feedback = UpdateTrackingNumber(order, orderRequest.TrackingNumber);
            if (!feedback.Success)
                return feedback;

            feedback = UpdateExternalId(order, orderRequest.ExternalId);
            if (!feedback.Success)
                return feedback;

            await UpdateOrderItemsAsync(order, orderRequest.OrderItems);

            UpdateStatusAndOrderTotal(order, orderRequest.OrderStatus);

            return ActionFeedback.Succeed();
        }

        private ActionFeedback UpdateExternalId(Order order, string externalId)
        {
            var isExisted = externalId.HasValue() 
                && _orderRepository.QueryAsNoTracking().Any(item => item.ExternalId == externalId && item.Id != order.Id);

            if (isExisted)
                return ActionFeedback.Fail("ExternalId has been used!");

            order.ExternalId = externalId;

            return ActionFeedback.Succeed();
        }

        private ActionFeedback UpdateTrackingNumber(Order order, string trackingNumber)
        {
            var isExisted = trackingNumber.HasValue() 
                && _orderRepository.QueryAsNoTracking().Any(item => item.TrackingNumber == trackingNumber && item.Id != order.Id);

            if (isExisted)
                return ActionFeedback.Fail("Tracking number has been used!");

            order.TrackingNumber = trackingNumber;

            return ActionFeedback.Succeed();
        }

        private void UpdateStatusAndOrderTotal(Order order, OrderStatus status)
        {
            order.OrderStatus = status;
            if (status == OrderStatus.Cancelled)
            {
                ResetOrderItemQuantities(order);
            }
            if (status == OrderStatus.Complete)
            {
                order.CompletedOn = DateTimeOffset.Now;
            }
            CalculateOrderTotal(order);
        }

        private void ResetOrderItemQuantities(Order order)
        {
            foreach (var orderItem in order.OrderItems)
            {
                orderItem.Product.Stock += orderItem.Quantity;
                orderItem.Quantity = 0;
            }
            order.Discount = 0;
        }

        private void CalculateOrderTotal(Order order)
        {
            order.SubTotal = order.OrderItems.Sum(item => item.SubTotal);
            order.OrderTotal = order.SubTotal + order.ShippingAmount - order.Discount;
            order.OrderTotalCost = order.OrderItems.Sum(item => item.SubTotalCost) + order.ShippingCost;
        }

        private async Task UpdateOrderItemsAsync(Order order, IEnumerable<OrderItemVm> orderItems)
        {
            if (order.OrderItems.Any())
            {
                foreach (var orderItem in order.OrderItems)
                {
                    var product = orderItem.Product;
                    product.Stock += orderItem.Quantity;
                }

                order.OrderItems.Clear();
            }

            foreach (var item in orderItems)
            {
                // Update product stock
                var product = await _productRepo.Query().FirstOrDefaultAsync(i => i.Id == item.ProductId);

                if (product == null)
                    continue;

                product.Stock -= item.Quantity;
                var orderItem = new OrderItem
                {
                    Product = product,
                    ProductPrice = item.ProductPrice,
                    Quantity = item.Quantity
                };
                order.AddOrderItem(orderItem);
            }
        }

        private bool CanEditOrder(Order order)
        {
            return _httpContextAccessor.HttpContext.User.IsInRole(RoleName.Admin)
                || order.OrderStatus != OrderStatus.Complete
                || order.CompletedOn > DateTimeOffset.Now.AddDays(-1);
        }
    }
}
