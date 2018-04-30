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
        Task<(bool, string)> CreateOrderAsync(OrderFormVm orderRequest);

        /// <summary>
        /// Get order detail for editting
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<(OrderFormVm, string)> GetOrder(long orderId);

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
