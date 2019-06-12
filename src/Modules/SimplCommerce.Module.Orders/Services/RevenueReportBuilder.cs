using System;
using System.Collections.Generic;
using System.Linq;
using SimplCommerce.Module.Orders.Models;

namespace SimplCommerce.Module.Orders.Services
{
    public class RevenueReportBuilder
    {
        private const string MonthFormat = "MM/yyyy";

        private readonly IReadOnlyCollection<Order> _orders;
        private readonly DateTimeOffset _from;
        private readonly DateTimeOffset _to;
        private readonly int _diffMonths;

        public RevenueReportBuilder(IReadOnlyCollection<Order> orders)
        {
            _orders = orders.OrderBy(item => item.CompletedOn).ToList();
            _from = _orders.First().CompletedOn.Value;
            _to = _orders.Last().CompletedOn.Value;
            _diffMonths = (_to.Month + _to.Year * 12) - (_from.Month + _from.Year * 12);
        }

        public IList<decimal> SubTotals { get; } = new List<decimal>();
        public IList<decimal> Totals { get; } = new List<decimal>();
        public IList<decimal> Costs { get; } = new List<decimal>();
        public IList<decimal> Profits { get; } = new List<decimal>();
        public IList<string> Months { get; } = new List<string>();

        public void EvaluateSubTotals()
        {
            Months.Clear();
            Totals.Clear();

            for (var index = 0; index <= _diffMonths; index++)
            {
                var period = _from.AddMonths(index);
                var ordersInMonth = _orders.Where(IsInMonth(period.Month));

                if (!ordersInMonth.Any())
                    continue;

                Months.Add(period.ToString(MonthFormat));
                SubTotals.Add(ordersInMonth.Sum(order => order.SubTotal));
            }
        }

        public void EvaluateTotals()
        {
            if (!SubTotals.Any()) // SubTotals need to be evaluated beforehand
                EvaluateSubTotals();

            Totals.Clear();

            for (var index = 0; index <= _diffMonths; index++)
            {
                var period = _from.AddMonths(index);
                var ordersInMonth = _orders.Where(IsInMonth(period.Month));

                if (!ordersInMonth.Any())
                    continue;

                Totals.Add(ordersInMonth.Sum(order => order.OrderTotal));
            }
        }

        public void EvaluateCostsAndProfits()
        {
            if (!Totals.Any()) // Totals need to be evaluated beforehand
                EvaluateTotals();

            Costs.Clear();
            Profits.Clear();

            for (var index = 0; index <= _diffMonths; index++)
            {
                var period = _from.AddMonths(index);
                var ordersInMonth = _orders.Where(IsInMonth(period.Month));

                if (!ordersInMonth.Any())
                    continue;

                Costs.Add(ordersInMonth.Sum(order => order.OrderTotalCost));
                Profits.Add(Totals[index] - Costs[index]);
            }
        }

        private static Func<Order, bool> IsInMonth(int month) => 
            order => order.CompletedOn.HasValue && order.CompletedOn.Value.Month == month;
    }
}
