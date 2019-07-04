using System.Collections.Generic;
using System.Threading.Tasks;
using SimplCommerce.Module.Core.Services.Dtos;

namespace SimplCommerce.Module.Core.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> SearchAsync(string query);
        Task<long> GetCustomerIdByPhoneAsync(string phone);
        Task<long> CreateCustomerAsync(CreateCustomerDto cusomter);
    }
}
