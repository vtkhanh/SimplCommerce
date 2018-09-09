using System.Threading.Tasks;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.ViewModels;

namespace SimplCommerce.Module.Orders.Services
{
    public interface IOrderService
    {
        /// <summary>
        /// Orders created by admins
        /// </summary>
        /// <param name="orderRequest">order detail</param>
        /// <returns>bool: success/fail; string: errorMessage</returns>
        Task<(long, string)> CreateOrderAsync(OrderFormVm orderRequest);

        /// <summary>
        /// Order updated by admins
        /// </summary>
        /// <param name="orderRequest"></param>
        /// <returns></returns>
        Task<(bool, string)> UpdateOrderAsync(OrderFormVm orderRequest);

        /// <summary>
        /// Update order tracking number
        /// </summary>
        /// <param name="trackingNumber"></param>
        /// <returns></returns>
        Task<(bool, string)> UpdateTrackingNumberAsync(long orderId, string trackingNumber);

        /// <summary>
        /// Update order status
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="statusId"></param>
        /// <returns></returns>
        Task<(bool, string)> UpdateStatusAsync(long orderId, OrderStatus status);

        /// <summary>
        /// Get order detail for editting
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<(OrderFormVm, string)> GetOrderAsync(long orderId);

        /// <summary>
        /// Create order for user from active cart
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<Order> CreateOrder(User user, string paymentMethod);

        Task<Order> CreateOrder(User user, string paymentMethod, string shippingMethod, Address billingAddress, Address shippingAddress);

        Task<decimal> GetTax(long cartOwnerUserId, long countryId, long stateOrProvinceId);
    }
}
