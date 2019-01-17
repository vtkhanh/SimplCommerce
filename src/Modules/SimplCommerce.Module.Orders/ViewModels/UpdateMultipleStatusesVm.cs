using System.Collections.Generic;
using SimplCommerce.Module.Orders.Models;

namespace SimplCommerce.Module.Orders.ViewModels
{
    public class UpdateMultipleStatusesVm
    {
        public IList<long> OrderIds { get; set; }
        public OrderStatus Status { get; set; }
    }
}
