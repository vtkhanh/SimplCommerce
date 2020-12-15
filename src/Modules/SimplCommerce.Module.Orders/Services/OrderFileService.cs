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

        public SmartTableResult<GetOrderFileDto> Get(SmartTableParam param, bool includeDeleted = false)
        {
            var orderFiles = _orderFileRepo
                .QueryAsNoTracking()
                .Where(item => includeDeleted || !item.IsDeleted)
                .Include(item => item.CreatedBy)
                .Include(item => item.ImportResults);

            var result = orderFiles.ToSmartTableResult(
                param,
                file => new GetOrderFileDto
                {
                    Id = file.Id,
                    FileName = file.FileName,
                    CreatedOn = file.CreatedOn,
                    Status = file.Status.ToString(),
                    CreatedBy = file.CreatedBy.FullName,
                    ImportResultId = file.ImportResults.OrderByDescending(item => item.Id).First().Id,
                    SuccessCount = file.ImportResults.OrderByDescending(item => item.Id).First().SuccessCount,
                    FailureCount = file.ImportResults.OrderByDescending(item => item.Id).First().FailureCount
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
                Status = file.Status.ToString(),
                ReferenceFileName = file.ReferenceFileName
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

        public async Task<bool> DeleteAsync(long id)
        {
            var file = await _orderFileRepo.Query().SingleOrDefaultAsync(item => item.Id == id);
            if (file is null)
            {
                return false;
            }

            file.IsDeleted = true;
            await _orderFileRepo.SaveChangesAsync();

            return true;
        }

    }
}
