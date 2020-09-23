using System.Collections.Generic;
using System.Threading.Tasks;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.Services.Dtos;

namespace SimplCommerce.Module.Orders.Services
{
    internal interface IOrderImportService
    {
        Task<ImportResult> ImportAsync(IEnumerable<ImportingOrderDto> orders);
    }
}
