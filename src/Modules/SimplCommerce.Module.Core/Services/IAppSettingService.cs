using System.Threading.Tasks;
using SimplCommerce.Module.Core.Services.Dtos;

namespace SimplCommerce.Module.Core.Services
{
    public interface IAppSettingService
    {
        Task<AppSettingDto> GetAsync(string key);
    }
}
