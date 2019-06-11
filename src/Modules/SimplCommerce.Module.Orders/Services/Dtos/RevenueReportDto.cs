using System;
using System.Collections.Generic;
using System.Linq;
using SimplCommerce.Module.Orders.Models;

namespace SimplCommerce.Module.Orders.Services.Dtos
{
    internal class RevenueReportDto
    {
        private const string MonthFormat = "MM/yyyy";

        private readonly DateTime _from;
        private readonly DateTime _to;

        public RevenueReportDto(DateTime from, DateTime to) => (_from, _to) = (from, to);

        public IList<decimal> SubTotals { get; } = new List<decimal>();
        public IList<decimal> Totals { get; } = new List<decimal>();
        public IList<decimal> Costs { get; } = new List<decimal>();
        public IList<decimal> Profits { get; } = new List<decimal>();
        public IList<string> Months { get; } = new List<string>();

        public void AddSubTotals(IList<Order> orders)
        {
            Months.Clear();
            Totals.Clear();

            for (var period = _from; period <= _to; period = period.AddMonths(1))
            {
                var ordersInMonth = orders.Where(IsInMonth(period));

                if (!ordersInMonth.Any())
                    continue;

                Months.Add(ordersInMonth.First().CompletedOn?.ToString(MonthFormat));
                SubTotals.Add(ordersInMonth.Sum(order => order.SubTotal));
            }
        }

        public void AddTotals(IList<Order> orders)
        {
            if (!SubTotals.Any()) // SubTotals need to be added beforehand
                return;

            Totals.Clear();

            for (var period = _from; period <= _to; period = period.AddMonths(1))
            {
                var ordersInMonth = orders.Where(IsInMonth(period));

                if (!ordersInMonth.Any())
                    continue;

                Totals.Add(ordersInMonth.Sum(order => order.OrderTotal));
            }
        }

        public void AddCostsAndProfits(IList<Order> orders)
        {
            if (!Totals.Any()) // SubTotals need to be added beforehand
                return;

            Costs.Clear();
            Profits.Clear();

            var index = 0;
            for (var period = _from; period <= _to; period = period.AddMonths(1))
            {
                var ordersInMonth = orders.Where(IsInMonth(period));

                if (!ordersInMonth.Any())
                    continue;

                Costs.Add(ordersInMonth.Sum(order => order.OrderTotalCost));
                Profits.Add(Totals[index] - Costs[index]);

                index++;
            }
        }

        private static Func<Order, bool> IsInMonth(DateTime period) => 
            order => order.CompletedOn.HasValue && order.CompletedOn.Value.Month == period.Month;

    }
}
