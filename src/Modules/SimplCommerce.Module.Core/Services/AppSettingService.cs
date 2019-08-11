using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Core.Services.Dtos;

namespace SimplCommerce.Module.Core.Services
{
    public class AppSettingService : IAppSettingService
    {
        private readonly IRepository<AppSetting> _appSettingRepo;

        public AppSettingService(IRepository<AppSetting> appSettingRepo)
        {
            _appSettingRepo = appSettingRepo;
        }

        public async Task<AppSettingDto> GetAsync(string key)
        {
            var appSetting = await _appSettingRepo
                .QueryAsNoTracking()
                .SingleOrDefaultAsync(setting => setting.Key == key);

            return AppSettingDto.Create(appSetting);
        }
    }
}
