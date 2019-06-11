using System;
using System.Collections.Generic;
using System.IO;
using SimplCommerce.Module.Orders.Services.Dtos;

namespace SimplCommerce.Module.Orders.Services
{
    internal class OrderFileParser : IOrderFileParser
    {
        public IEnumerable<ImportingOrderDto> Parse(Stream file)
        {
            throw new NotImplementedException();
        }
    }
}
