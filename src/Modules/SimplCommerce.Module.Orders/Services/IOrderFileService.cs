using System.Collections.Generic;
using System.Threading.Tasks;
using SimplCommerce.Module.Orders.Services.Dtos;

namespace SimplCommerce.Module.Orders.Services
{
    public interface IOrderFileService
    {
        Task SaveAsync(SaveOrderFileDto request);
        Task<IList<GetOrderFileDto>> GetAsync();
    }
}
