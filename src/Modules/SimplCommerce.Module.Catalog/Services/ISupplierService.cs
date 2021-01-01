using System.Collections.Generic;
using System.Threading.Tasks;
using SimplCommerce.Module.Catalog.Services.Dtos;

namespace SimplCommerce.Module.Catalog.Services
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierDto>> GetAllAsync();

        Task<SupplierDto> GetAsync(long id);

        Task<long> CreateAsync(SupplierDto supplier);

        Task<bool> UpdateAsync(SupplierDto supplier);

        Task<bool> DeleteAsync(long id);
    }
}
