using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Web.SmartTable;
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

        public SmartTableResult<GetOrderFileDto> Get(SmartTableParam param)
        {
            var orderFiles = _orderFileRepo
                .QueryAsNoTracking()
                .Include(item => item.CreatedBy)
                .Include(item => item.ImportResults)
                .Select(file => new
                {
                    file.Id,
                    file.FileName,
                    file.CreatedOn,
                    Status = file.Status.ToString(),
                    CreatedBy = file.CreatedBy.FullName,
                    ImportResult = file.ImportResults.OrderByDescending(item => item.Id).FirstOrDefault()
                });

            var result = orderFiles.ToSmartTableResult(
                param,
                file => new GetOrderFileDto
                {
                    Id = file.Id,
                    FileName = file.FileName,
                    CreatedOn = file.CreatedOn,
                    Status = file.Status.ToString(),
                    CreatedBy = file.CreatedBy,
                    ImportResultId = file.ImportResult is object ? file.ImportResult.Id : 0,
                    SuccessCount = file.ImportResult is object ? file.ImportResult.SuccessCount : 0,
                    FailureCount = file.ImportResult is object ? file.ImportResult.FailureCount : 0,
                }
            );

            return result;
        }

        public async Task<GetOrderFileDto> GetByIdAsync(long id)
        {
            var file = await _orderFileRepo.QueryAsNoTracking().FirstOrDefaultAsync(item => item.Id == id);

            if (file is null)
            {
                return null;
            }

            return new GetOrderFileDto
            {
                Id = file.Id,
                FileName = file.FileName,
                CreatedOn = file.CreatedOn,
                Status = file.Status.ToString()
            };
        }

        public async Task<long> SaveAsync(SaveOrderFileDto request)
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

            return model.Id;
        }

        public async Task<bool> UpdateStatusAsync(long id, ImportFileStatus status)
        {
            var file = await _orderFileRepo.Query().FirstOrDefaultAsync(item => item.Id == id);

            if (file is null)
            {
                return false;
            }

            file.Status = status;
            await _orderFileRepo.SaveChangesAsync();

            return true;
        }
    }
}
