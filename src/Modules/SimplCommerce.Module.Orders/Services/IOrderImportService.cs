﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SimplCommerce.Module.Orders.Services.Dtos;

namespace SimplCommerce.Module.Orders.Services
{
    internal interface IOrderImportService
    {
        Task<bool> ImportAsync(long importedById, long orderFileId, IEnumerable<ImportingOrderDto> orders);
    }
}