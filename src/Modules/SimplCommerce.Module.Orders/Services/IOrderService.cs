using System.Collections.Generic;
using System.Threading.Tasks;
using SimplCommerce.Infrastructure.ResultTypes;
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
        Task<ActionFeedback<long>> CreateOrderAsync(OrderFormVm orderRequest);

        /// <summary>
        /// Order updated by admins
        /// </summary>
        /// <param name="orderRequest"></param>
        /// <returns></returns>
        Task<ActionFeedback> UpdateOrderAsync(OrderFormVm orderRequest);

        /// <summary>
        /// Update order status, tracking number, and payment provider
        /// </summary>
        /// <param name="orderRequest"></param>
        /// <returns></returns>
        Task<ActionFeedback> UpdateOrderStateAsync(OrderFormVm orderRequest);

        /// <summary>
        /// Update order tracking number
        /// </summary>
        /// <param name="trackingNumber"></param>
        /// <returns></returns>
        Task<ActionFeedback> UpdateTrackingNumberAsync(long orderId, string trackingNumber);

        /// <summary>
        /// Update order status
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="statusId"></param>
        /// <returns></returns>
        Task<(GetOrderVm, string)> UpdateStatusAsync(long orderId, OrderStatus status);

        /// <summary>
        /// Update status of multiple orders
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<(IList<GetOrderVm>, string)> UpdateStatusesAsync(IList<long> orderIds, OrderStatus status);

        /// <summary>
        /// Get order detail for editting
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<ActionFeedback<GetOrderVm>> GetOrderAsync(long orderId);

        /// <summary>
        /// Create order for user from active cart
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<Order> CreateOrder(User user, string paymentMethod);

        Task<Order> CreateOrder(User user, string paymentMethod, string shippingMethod, Address billingAddress, Address shippingAddress);

        Task<decimal> GetTax(long cartOwnerUserId, long countryId, long stateOrProvinceId);

        /// <summary>
        /// Get User Id of the owner of an order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<long> GetOrderOwnerIdAsync(long orderId);
    }
}
