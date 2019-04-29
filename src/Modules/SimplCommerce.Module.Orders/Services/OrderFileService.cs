using System;
using System.Threading.Tasks;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.Models.Enums;
using SimplCommerce.Module.Orders.Services.Dtos;

namespace SimplCommerce.Module.Orders.Services
{
    public class OrderFileService : IOrderFileService
    {
        private readonly IRepository<OrderFile> _importFileRepo;

        public OrderFileService(IRepository<OrderFile> importFileRepo)
        {
            _importFileRepo = importFileRepo;
        }

        public async Task SaveAsync(SaveOrderFileDto request)
        {
            var model = new OrderFile
            {
                FileName = request.FileName,
                ReferenceFileName = request.ReferenceFileName,
                CreatedById = request.CreatedById,
                CreatedOn = request.CreatedOn,
                Status = ImportFileStatus.Pending
            };
            _importFileRepo.Add(model);
            await _importFileRepo.SaveChangesAsync();
        }
    }
}
