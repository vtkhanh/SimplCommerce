using System.Collections.Generic;
using System.IO;
using SimplCommerce.Infrastructure.ResultTypes;
using SimplCommerce.Module.Orders.Services.Dtos;

namespace SimplCommerce.Module.Orders.Services
{
    internal interface IOrderFileParser
    {
        ActionFeedback<IEnumerable<ImportingOrderDto>> Parse(Stream file);
    }
}
