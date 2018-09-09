using SimplCommerce.Module.Orders.Models;

namespace SimplCommerce.Module.Orders.ViewModels
{
    public class OrderUpdateVm
    {
        public long OrderId { get; set; }

        public string TrackingNumber { get; set; }

        public OrderStatus Status { get; set; }
    }
}
