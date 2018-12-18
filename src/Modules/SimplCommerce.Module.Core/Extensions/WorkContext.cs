using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using SimplCommerce.Module.Core.Models;

namespace SimplCommerce.Module.Core.Extensions
{
    public class WorkContext : IWorkContext
    {
        private User _currentUser;
        private UserManager<User> _userManager;
        private HttpContext _httpContext;

        public WorkContext(UserManager<User> userManager, IHttpContextAccessor contextAccessor)
        {
            _userManager = userManager;
            _httpContext = contextAccessor.HttpContext;
        }

        public async Task<User> GetCurrentUser()
        {
            if (_currentUser != null)
            {
                return _currentUser;
            }

            // On external login callback Identity.IsAuthenticated = true. But it's an external claim principal
            // Login by google, get _userManager.GetUserAsync from ClaimsPrincipal throw exception becasue the UserIdClaimType has value but too big.
            if (_httpContext.User.Identity.AuthenticationType == "Identity.Application")
            {
                var contextUser = _httpContext.User;
                _currentUser = await _userManager.GetUserAsync(contextUser);
                if (_currentUser != null)
                {
                    return _currentUser;
                }
            }

            _currentUser = CreateGuest();

            return _currentUser;
        }

        private User CreateGuest()
        {
            Guid? userGuid = Guid.NewGuid();
            var dummyEmail = $"{userGuid}@guest.simplcommerce.com";
            var user = new User
            {
                FullName = "Guest",
                UserGuid = userGuid.Value,
                Email = dummyEmail,
                UserName = dummyEmail
            };
            return user;
        }
    }
}
