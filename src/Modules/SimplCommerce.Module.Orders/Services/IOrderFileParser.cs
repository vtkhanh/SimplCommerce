using System.Collections.Generic;
using System.IO;
using SimplCommerce.Module.Orders.Services.Dtos;

namespace SimplCommerce.Module.Orders.Services
{
    internal interface IOrderFileParser
    {
        IEnumerable<ImportingOrderDto> Parse(Stream file);
    }
}
