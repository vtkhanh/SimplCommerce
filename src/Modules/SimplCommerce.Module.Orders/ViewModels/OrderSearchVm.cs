using System;
using SimplCommerce.Module.Orders.Models;

namespace SimplCommerce.Module.Orders.ViewModels
{
    public class OrderSearchVm
    {
        private readonly dynamic _predicateObject;

        public OrderSearchVm(dynamic predicateObject) => _predicateObject = predicateObject;

        public long? Id => (long?)_predicateObject.Id;

        public OrderStatus? Status => (OrderStatus?)_predicateObject.Status;

        public string CustomerName => (string)_predicateObject.CustomerName;

        public string TrackingNumber => (string)_predicateObject.TrackingNumber;

        public string CreatedBy => (string)_predicateObject.CreatedBy;

        public DateTimeOffset? CreatedBefore
        {
            get
            {
                var createdBefore = (DateTimeOffset?)_predicateObject.CreatedOn?.before;
                if (!createdBefore.HasValue && CreatedAfter.HasValue)
                {
                    createdBefore = GetLastDayOfMonth(CreatedAfter.Value);
                }
                return createdBefore;
            }
        }

        public DateTimeOffset? CreatedAfter => (DateTimeOffset?)_predicateObject.CreatedOn?.after;

        public DateTimeOffset? CompletedBefore
        {
            get
            {
                var completedBefore = (DateTimeOffset?)_predicateObject.CompletedOn?.before;
                if (!completedBefore.HasValue && CompletedAfter.HasValue)
                {
                    completedBefore = GetLastDayOfMonth(CompletedAfter.Value);
                }
                return completedBefore;
            }
        }

        public DateTimeOffset? CompletedAfter => (DateTimeOffset?)_predicateObject.CompletedOn?.after;

        private DateTime GetLastDayOfMonth(DateTimeOffset date)
        {
            var year = date.Year;
            var month = date.Month;
            var lastDay = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(year, month), 23, 59, 59);
            return lastDay;
        }
    }
}
