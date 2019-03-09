using System;

namespace SimplCommerce.Module.Catalog.ViewModels
{
    public class PeriodVm
    {
        public DateTimeOffset? Before { get; set; }

        public DateTimeOffset? After { get; set; }
    }
}
