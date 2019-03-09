using System;

namespace SimplCommerce.Module.Catalog.ViewModels
{
    public class SearchProductParametersVm
    {
        public string Name { get; set; }

        public string Sku { get; set; }

        public long? VendorId { get; set; }

        public bool? HasOptions { get; set; }

        public bool? InStock { get; set; }

        public bool? IsVisibleIndividually { get; set; }

        public bool? IsPublished { get; set; }

        public PeriodVm CreatedOn { get; set; }

        public DateTimeOffset? CreatedBefore => CreatedOn?.Before;

        public DateTimeOffset? CreatedAfter => CreatedOn?.After;

        public bool CanManageOrder { get; set; }
    }
}
