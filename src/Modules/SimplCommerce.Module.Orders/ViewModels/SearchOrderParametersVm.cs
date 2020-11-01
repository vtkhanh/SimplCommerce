using System;
using SimplCommerce.Module.Orders.Models;

namespace SimplCommerce.Module.Orders.ViewModels
{
    public class SearchOrderParametersVm
    {
        public long? Id { get; set; }

        public OrderStatus? Status { get; set; }

        public string CustomerName { get; set; }

        public string TrackingNumber { get; set; }

        public string ExternalId { get; set; }

        public string CreatedBy { get; set; }

        public PeriodVm CreatedOn { get; set; }

        public PeriodVm CompletedOn { get; set; }

        public DateTimeOffset? CreatedBefore
        {
            get
            {
                var createdBefore = CreatedOn?.Before;
                if (!createdBefore.HasValue && CreatedAfter.HasValue)
                {
                    createdBefore = CreatedAfter.Value.AddDays(30);
                }
                return createdBefore;
            }
        }

        public DateTimeOffset? CreatedAfter => CreatedOn?.After;

        public DateTimeOffset? CompletedBefore
        {
            get
            {
                var completedBefore = CompletedOn?.Before;
                if (!completedBefore.HasValue && CompletedAfter.HasValue)
                {
                    completedBefore = CompletedAfter.Value.AddDays(30);
                }
                return completedBefore;
            }
        }

        public DateTimeOffset? CompletedAfter => CompletedOn?.After;

        public bool CanManageOrder { get; set; }

        public long? UserVendorId { get; set; }
    }
}
