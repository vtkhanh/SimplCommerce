﻿using System.Threading.Tasks;
using SimplCommerce.Module.Orders.Services.Dtos;

namespace SimplCommerce.Module.Orders.Services
{
    public interface IImportResultService
    {
        Task<ImportResultDto> GetAsync(long id);
    }
}
