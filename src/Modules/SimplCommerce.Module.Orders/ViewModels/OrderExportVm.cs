using SimplCommerce.Module.Orders.Models;

namespace SimplCommerce.Module.Orders.ViewModels
{
    public class OrderExportVm
    {
        public long Id { get; set; }

        public OrderStatus Status { get; set; }

        public string CustomerName { get; set; }

        public string TrackingNumber { get; set; }

        public string CreatedBy { get; set; }

        public decimal Cost { get; set; }

        public decimal Total { get; set; }

        public string CreatedOn { get; set; }

        public string CompletedOn { get; set; }
    }
}
