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
                    createdBefore = GetLastDayOfMonth(CreatedAfter.Value);
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
                    completedBefore = GetLastDayOfMonth(CompletedAfter.Value);
                }
                return completedBefore;
            }
        }

        public DateTimeOffset? CompletedAfter => CompletedOn?.After;

        public bool CanManageOrder { get; set; }

        public long? UserVendorId { get; set; }

        private DateTime GetLastDayOfMonth(DateTimeOffset date)
        {
            var year = date.Year;
            var month = date.Month;
            var lastDay = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(year, month), 23, 59, 59);
            return lastDay;
        }
    }
}
