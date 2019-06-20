using System.Collections.Generic;
using SimplCommerce.Module.Orders.Services.Dtos;

namespace SimplCommerce.Module.Orders.Services
{
    internal interface IOrderImportService
    {
        bool Import(IEnumerable<ImportingOrderDto> orders);
    }
}
