using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Core.Extensions.Constants;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Core.Services.Dtos;
using SimplCommerce.Module.Core.ViewModels;

namespace SimplCommerce.Module.Core.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IRepository<User> _userRepo;

        public UserService(UserManager<User> userManager, IRepository<User> userRepo) =>
            (_userManager, _userRepo) = (userManager, userRepo);

        public async Task<(IdentityResult, long)> CreateUserAsync(UserForm model)
        {
            var user = new User();

            SetBasicInfo(user, model);
            AddOrUpdateAddress(user, model.Address);
            AddRoles(user, model.RoleIds);
            AddCustomerGroups(user, model.CustomerGroupIds);

            var result = await _userManager.CreateAsync(user, model.Password);

            return (result, user.Id);
        }

        public async Task<IdentityResult> UpdateUserAsync(long userId, UserForm model)
        {
            var user = await _userRepo.Query()
                .Include(x => x.DefaultShippingAddress)
                .Include(x => x.Roles)
                .Include(x => x.CustomerGroups)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Cannot find user with id {userId}" });
            }

            SetBasicInfo(user, model);
            AddOrUpdateAddress(user, model.Address);
            AddOrDeleteRoles(model, user);
            AddOrDeleteCustomerGroups(model, user);

            var result = await _userManager.UpdateAsync(user);

            return result;
        }

        public async Task<IList<UserDto>> GetSellersAsync() =>
            await _userRepo.QueryAsNoTracking()
                .Where(user => user.Roles.Any(userRole => userRole.RoleId == (long) RoleId.Seller))
                .Select(item => new UserDto { Id = item.Id, FullName = item.FullName })
                .ToListAsync();


        private void SetBasicInfo(User user, UserForm model)
        {
            user.Email = model.Email;
            user.UserName = model.Email;
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.VendorId = model.VendorId;
            user.Link = model.Link;
        }

        private void AddOrUpdateAddress(User user, string address)
        {
            if (!user.DefaultShippingAddressId.HasValue) // Add a new one
            {
                if (address.HasValue())
                {
                    user.DefaultShippingAddress = new Address
                    {
                        AddressLine1 = address
                    };
                }
            }
            else // Update
            {
                user.DefaultShippingAddress.AddressLine1 = address;
            }
        }

        private void AddOrDeleteRoles(UserForm model, User user)
        {
            foreach (var roleId in model.RoleIds)
            {
                if (user.Roles.Any(x => x.RoleId == roleId))
                {
                    continue;
                }

                var userRole = new UserRole
                {
                    RoleId = roleId,
                    User = user
                };
                user.Roles.Add(userRole);
            }

            var deletedUserRoles =
                user.Roles
                    .Where(userRole => !model.RoleIds.Contains(userRole.RoleId))
                    .ToList();

            foreach (var deletedUserRole in deletedUserRoles)
            {
                deletedUserRole.User = null;
                user.Roles.Remove(deletedUserRole);
            }
        }

        private void AddOrDeleteCustomerGroups(UserForm model, User user)
        {
            foreach (var customergroupId in model.CustomerGroupIds)
            {
                if (user.CustomerGroups.Any(x => x.CustomerGroupId == customergroupId))
                {
                    continue;
                }

                var userCustomerGroup = new UserCustomerGroup
                {
                    CustomerGroupId = customergroupId,
                    User = user
                };
                user.CustomerGroups.Add(userCustomerGroup);
            }

            var deletedUserCustomerGroups =
                user.CustomerGroups
                    .Where(userCustomerGroup => !model.CustomerGroupIds.Contains(userCustomerGroup.CustomerGroupId))
                    .ToList();

            foreach (var deletedUserCustomerGroup in deletedUserCustomerGroups)
            {
                deletedUserCustomerGroup.User = null;
                user.CustomerGroups.Remove(deletedUserCustomerGroup);
            }
        }

        private void AddCustomerGroups(User user, IList<long> customerGroupIds)
        {
            if (customerGroupIds == null) return;

            foreach (var customergroupId in customerGroupIds)
            {
                var customerGroup = new UserCustomerGroup
                {
                    CustomerGroupId = customergroupId
                };
                user.CustomerGroups.Add(customerGroup);
            }
        }

        private void AddRoles(User user, IList<long> roleIds)
        {
            if (roleIds == null) return;

            foreach (var roleId in roleIds)
            {
                var role = new UserRole
                {
                    RoleId = roleId
                };

                user.Roles.Add(role);
                role.User = user;
            }
        }

    }
}
