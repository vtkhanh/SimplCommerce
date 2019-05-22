using System.Threading.Tasks;
using SimplCommerce.Infrastructure.Web.SmartTable;
using SimplCommerce.Module.Orders.Models.Enums;
using SimplCommerce.Module.Orders.Services.Dtos;

namespace SimplCommerce.Module.Orders.Services
{
    public interface IOrderFileService
    {
        Task<GetOrderFileDto> GetByIdAsync(long id);
        SmartTableResult<GetOrderFileDto> Get(SmartTableParam param);
        Task<long> SaveAsync(SaveOrderFileDto request);
        Task<bool> UpdateStatusAsync(long id, ImportFileStatus status);
    }
}
