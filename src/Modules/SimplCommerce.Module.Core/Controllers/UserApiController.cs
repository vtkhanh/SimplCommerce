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
using SimplCommerce.Module.Core.Services;
using AutoMapper;
using SimplCommerce.Module.Core.Extensions.Constants;
using SimplCommerce.Infrastructure;
using SimplCommerce.Module.Core.Services.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SimplCommerce.Module.Core.Controllers
{
    [Authorize(Policy = Policy.CanManageUser)]
    [Route("api/users")]
    public class UserApiController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepository<User> _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;

        public UserApiController(
            IMapper mapper,
            IRepository<User> userRepo,
            UserManager<User> userManager,
            IUserService userService) =>
            (_mapper, _userRepository, _userManager, _userService) =
                (mapper, userRepo, userManager, userService);

        [HttpPost("list")]
        public IActionResult List([FromBody] SmartTableParam param)
        {
            var query = _userRepository.Query()
                .Include(x => x.Roles).ThenInclude(x => x.Role)
                .Include(x => x.CustomerGroups).ThenInclude(x => x.CustomerGroup)
                .Where(x => !x.IsDeleted);

            if (param.Search.PredicateObject != null)
            {
                dynamic search = param.Search.PredicateObject;
                string phoneNumber = search.PhoneNumber;
                string fullName = search.FullName;
                string roleName = search.Role;
                string customerGroupName = search.CustomerGroup;
                DateTimeOffset? before = search.CreatedOn?.before;
                DateTimeOffset? after = search.CreatedOn?.after;
                query = query
                    .WhereIf(phoneNumber.HasValue(), item => item.PhoneNumber.Contains(phoneNumber))
                    .WhereIf(fullName.HasValue(), item => item.FullName.Contains(fullName))
                    .WhereIf(roleName.HasValue(), item => item.Roles.Any(role => role.Role.Name.Contains(roleName)))
                    .WhereIf(customerGroupName.HasValue(), item => item.CustomerGroups.Any(cg => cg.CustomerGroup.Name.Contains(customerGroupName)))
                    .WhereIf(before.HasValue, item => item.CreatedOn <= before)
                    .WhereIf(after.HasValue, item => item.CreatedOn >= after);
            }

            var users = query.ToSmartTableResult(
                param,
                user => new
                {
                    user.Id,
                    user.Email,
                    user.FullName,
                    user.PhoneNumber,
                    user.CreatedOn,
                    Roles = string.Join(", ", user.Roles.Select(x => x.Role.Name)),
                    CustomerGroups = string.Join(", ", user.CustomerGroups.Select(x => x.CustomerGroup.Name)),
                    CanEdit = CanEditUser(user)
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

            if (user == null)
            {
                return NotFound();
            }

            var model = _mapper.Map<UserForm>(user);

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

        [HttpGet("seller-list")]
        public async Task<IActionResult> GetSellers()
        {
            var sellers = await _userService.GetSellersAsync();
            var selectList = sellers.Select(item => new SelectListItem { Value = item.Id.ToString(), Text = item.FullName });
            selectList = selectList.Prepend(new SelectListItem { Value = null, Text = "All" });
            return Json(selectList);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private bool CanEditUser(User user) => User.IsInRole(RoleName.Admin) || !user.Roles.Any(item => item.Role.Name == RoleName.Admin);
    }
}
