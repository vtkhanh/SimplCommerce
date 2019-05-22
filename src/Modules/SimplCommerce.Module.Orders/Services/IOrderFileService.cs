using System.Threading.Tasks;
using SimplCommerce.Infrastructure.Web.SmartTable;
using SimplCommerce.Module.Orders.Services.Dtos;

namespace SimplCommerce.Module.Orders.Services
{
    public interface IOrderFileService
    {
        Task SaveAsync(SaveOrderFileDto request);
        SmartTableResult<GetOrderFileDto> GetOrderFiles(SmartTableParam param);
    }
}
