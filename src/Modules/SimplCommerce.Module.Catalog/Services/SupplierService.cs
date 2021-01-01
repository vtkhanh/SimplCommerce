using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Catalog.Models;
using SimplCommerce.Module.Catalog.Services.Dtos;

namespace SimplCommerce.Module.Catalog.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly IRepository<Supplier> _repository;

        public SupplierService(IRepository<Supplier> repository) => _repository = repository;

        public async Task<long> CreateAsync(SupplierDto dto)
        {
            var supplier = new Supplier
            {
                Name = dto.Name,
                Phone = dto.Phone,
                Address = dto.Address
            };

            _repository.Add(supplier);
            await _repository.SaveChangesAsync();

            return supplier.Id;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var supplier = await _repository.Query().SingleAsync(item => item.Id == id);
            supplier.IsDeleted = true;
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<SupplierDto>> GetAllAsync()
        {
            var suppliers = await _repository.QueryAsNoTracking().Where(x => !x.IsDeleted).ToListAsync();
            var result = suppliers.Select(item => new SupplierDto
            {
                Id = item.Id,
                Name = item.Name,
                Phone = item.Phone,
                Address = item.Address
            });

            return result;
        }

        public async Task<SupplierDto> GetAsync(long id)
        {
            var supplier = await _repository.QueryAsNoTracking().SingleAsync(item => item.Id == id && !item.IsDeleted);
            var result = new SupplierDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Phone = supplier.Phone,
                Address = supplier.Address
            };

            return result;
        }

        public async Task<bool> UpdateAsync(SupplierDto dto)
        {
            var supplier = await _repository.Query().SingleAsync(item => item.Id == dto.Id && !item.IsDeleted);
            supplier.Name = dto.Name;
            supplier.Phone = dto.Phone;
            supplier.Address = dto.Address;
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
