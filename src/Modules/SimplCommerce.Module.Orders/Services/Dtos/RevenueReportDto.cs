using System;
using System.Collections.Generic;
using System.Linq;
using SimplCommerce.Module.Orders.Models;

namespace SimplCommerce.Module.Orders.Services.Dtos
{
    public class RevenueReportDto
    {
        private DateTime _from;
        private DateTime _to;

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
                var ordersInMonth = orders.Where(order => order.CreatedOn.Month == period.Month);

                if (!ordersInMonth.Any())
                    continue;

                Months.Add(ordersInMonth.First().CreatedOn.ToString("MM/yyyy"));
                SubTotals.Add(ordersInMonth.Sum(order => order.SubTotal));
            }
        }

        public void AddTotals(IList<Order> orders)
        {
            if (!SubTotals.Any()) // SubTotals need to be added beborehand
                return;

            Totals.Clear();

            for (var period = _from; period <= _to; period = period.AddMonths(1))
            {
                var ordersInMonth = orders.Where(order => order.CreatedOn.Month == period.Month);

                if (!ordersInMonth.Any())
                    continue;

                Totals.Add(ordersInMonth.Sum(order => order.OrderTotal));
            }
        }

        public void AddCostsAndProfits(IList<Order> orders)
        {
            if (!Totals.Any()) // SubTotals need to be added beborehand
                return;

            Costs.Clear();
            Profits.Clear();

            var index = 0;
            for (var period = _from; period <= _to; period = period.AddMonths(1))
            {
                var ordersInMonth = orders.Where(order => order.CreatedOn.Month == period.Month);

                if (!ordersInMonth.Any())
                    continue;

                Costs.Add(ordersInMonth.Sum(order => order.OrderTotalCost));
                Profits.Add(Totals[index] - Costs[index]);

                index++;
            }
        }
    }
}
