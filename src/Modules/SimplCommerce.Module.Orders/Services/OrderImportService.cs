using System;
using System.Collections.Generic;
using System.Linq;
using SimplCommerce.Module.Orders.Services.Dtos;

namespace SimplCommerce.Module.Orders.Services
{
    internal class OrderImportService : IOrderImportService
    {
        public bool Import(IEnumerable<ImportingOrderDto> orders)
        {
            return orders.Any();
        }
    }
}
