using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.Models.Enums;
using SimplCommerce.Module.Orders.Services.Dtos;

namespace SimplCommerce.Module.Orders.Services
{
    public class OrderFileService : IOrderFileService
    {
        private readonly IRepository<OrderFile> _orderFileRepo;

        public OrderFileService(IRepository<OrderFile> importFileRepo)
        {
            _orderFileRepo = importFileRepo;
        }

        public async Task<IList<GetOrderFileDto>> GetAsync()
        {
            return await _orderFileRepo
                .QueryAsNoTracking()
                .Select(file => new GetOrderFileDto
                {
                    Id = file.Id,
                    FileName = file.FileName,
                    CreatedOn = file.CreatedOn,
                    Status = file.Status.ToString(),
                    CreatedBy = file.CreatedBy.FullName
                })
                .ToListAsync();
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
            _orderFileRepo.Add(model);
            await _orderFileRepo.SaveChangesAsync();
        }
    }
}
