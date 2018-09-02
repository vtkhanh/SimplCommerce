using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Infrastructure.Web.SmartTable;
using SimplCommerce.Module.Core.ViewModels;
using SimplCommerce.Infrastructure;
using SimplCommerce.Module.Core.Services;

namespace SimplCommerce.Module.Core.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/users")]
    public class UserApiController : Controller
    {
        private readonly IRepository<User> _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;

        public UserApiController(IRepository<User> userRepo, 
            UserManager<User> userManager,
            IUserService userService
            ) =>
            (_userRepository, _userManager, _userService) = (userRepo, userManager, userService);

        [HttpPost("list")]
        public IActionResult List([FromBody] SmartTableParam param)
        {
            var query = _userRepository.Query()
                .Include(x => x.Roles)
                    .ThenInclude(x => x.Role)
                .Include(x => x.CustomerGroups)
                    .ThenInclude(x => x.CustomerGroup)
                .Where(x => !x.IsDeleted);

            if (param.Search.PredicateObject != null)
            {
                dynamic search = param.Search.PredicateObject;

                if (search.Email != null)
                {
                    string email = search.Email;
                    query = query.Where(x => x.Email.Contains(email));
                }

                if (search.FullName != null)
                {
                    string fullName = search.FullName;
                    query = query.Where(x => x.FullName.Contains(fullName));
                }

                if (search.Role != null)
                {
                    string roleName = search.Role;
                    query = ((from i in query
                              from p in i.Roles
                              where p.Role.Name.Contains(roleName)
                              select i) as IQueryable<User>);
                }

                if (search.CustomerGroup != null)
                {
                    string customerGroupName = search.CustomerGroup;
                    query = ((from i in query
                              from p in i.CustomerGroups
                              where p.CustomerGroup.Name.Contains(customerGroupName)
                              select i) as IQueryable<User>);
                }

                if (search.CreatedOn != null)
                {
                    if (search.CreatedOn.before != null)
                    {
                        DateTimeOffset before = search.CreatedOn.before;
                        query = query.Where(x => x.CreatedOn <= before);
                    }

                    if (search.CreatedOn.after != null)
                    {
                        DateTimeOffset after = search.CreatedOn.after;
                        query = query.Where(x => x.CreatedOn >= after);
                    }
                }
            }

            var users = query.ToSmartTableResult(
                param,
                user => new
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    CreatedOn = user.CreatedOn,
                    Roles = string.Join(", ", user.Roles.Select(x => x.Role.Name)),
                    CustomerGroups = string.Join(", ", user.CustomerGroups.Select(x => x.CustomerGroup.Name))
                });

            return Json(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var user = await _userRepository.Query()
                .Include(x => x.Roles)
                .Include(x => x.DefaultShippingAddress)
                .Include(x => x.CustomerGroups)
                .FirstOrDefaultAsync(x => x.Id == id);

            if(user == null)
            {
                return NotFound();
            }

            var model = new UserForm
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.DefaultShippingAddressId.HasValue ? user.DefaultShippingAddress.AddressLine1 : "",
                VendorId = user.VendorId,
                RoleIds = user.Roles.Select(x => x.RoleId).ToList(),
                CustomerGroupIds = user.CustomerGroups.Select(x => x.CustomerGroupId).ToList()
            };

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserForm model)
        {
            if (ModelState.IsValid)
            {
                var (result, userId) = await _userService.CreateUserAsync(model);

                if (result.Succeeded)
                {
                    return CreatedAtAction(nameof(Get), new { id = userId }, null);
                }

                AddErrors(result);
            }

            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] UserForm model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.UpdateUserAsync(id, model);

                if (result.Succeeded)
                {
                    return Accepted();
                }

                AddErrors(result);
            }

            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var user = await _userRepository.Query().FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsDeleted = true;
            await _userRepository.SaveChangesAsync();
            return NoContent();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
