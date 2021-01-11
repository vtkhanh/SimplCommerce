using System.Collections.Generic;
using System.Threading.Tasks;
using SimplCommerce.Module.Catalog.Services.Dtos;

namespace SimplCommerce.Module.Catalog.Services
{
    public interface IStockImportService
    {
        Task<IEnumerable<StockImportDto>> GetByProductAsync(long productId);

        Task<StockImportDto> GetAsync(long id);

        Task<long> CreateAsync(StockImportDto dto);

        Task<bool> UpdateAsync(StockImportDto dto);
    }
}
