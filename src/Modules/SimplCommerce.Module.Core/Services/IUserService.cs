using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SimplCommerce.Module.Core.Services.Dtos;
using SimplCommerce.Module.Core.ViewModels;

namespace SimplCommerce.Module.Core.Services
{
    public interface IUserService
    {
        Task<(IdentityResult, long)> CreateUserAsync(UserForm model);
        Task<IdentityResult> UpdateUserAsync(long userId, UserForm model);
        Task<IList<UserDto>> GetSellersAsync();
    }
}
