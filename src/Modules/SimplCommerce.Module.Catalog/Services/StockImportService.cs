using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Catalog.Models;
using SimplCommerce.Module.Catalog.Services.Dtos;

namespace SimplCommerce.Module.Catalog.Services
{
    public class StockImportService : IStockImportService
    {
        private IRepository<StockImport> _repository;

        public StockImportService(IRepository<StockImport> repository) => _repository = repository;

        public async Task<long> CreateAsync(StockImportDto dto)
        {
            var stockImport = new StockImport
            {
                ProductId = dto.ProductId,
                Date = dto.Date,
                SupplierId = dto.SupplierId,
                Quantity = dto.Quantity,
                Cost = dto.Cost,
                NewPrice = dto.NewPrice
            };
            _repository.Add(stockImport);
            await _repository.SaveChangesAsync();

            return stockImport.Id;
        }

        public async Task<IEnumerable<StockImportDto>> GetByProductAsync(long productId)
        {
            var entities = await _repository.QueryAsNoTracking()
                .Include(item => item.Supplier)
                .Where(item => item.ProductId == productId)
                .OrderByDescending(item => item.Date)
                .ToListAsync();
            var result = entities.Select(item => new StockImportDto
            {
                Date = item.Date,
                SupplierId = item.SupplierId,
                Quantity = item.Quantity,
                Cost = item.Cost,
                NewPrice = item.NewPrice,
                SupplierName = item.Supplier.Name
            });

            return result;
        }

        public async Task<StockImportDto> GetAsync(long id)
        {
            var entity = await _repository.QueryAsNoTracking().SingleAsync(item => item.Id == id);
            var result = new StockImportDto
            {
                ProductId = entity.ProductId,
                SupplierId = entity.SupplierId,
                Quantity = entity.Quantity,
                Cost = entity.Cost,
                NewPrice = entity.NewPrice
            };
            return result;
        }

        public async Task<bool> UpdateAsync(StockImportDto dto)
        {
            var entity = await _repository.Query().SingleAsync(item => item.Id == dto.Id);
            entity.SupplierId = dto.SupplierId;
            entity.Quantity = dto.Quantity;
            entity.Cost = dto.Cost;
            entity.NewPrice = dto.NewPrice;
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
