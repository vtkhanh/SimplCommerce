using System;

namespace SimplCommerce.Module.Orders.ViewModels
{
    public class PeriodVm
    {
        public DateTimeOffset? Before { get; set; }
        public DateTimeOffset? After { get; set; }
    }
}
