using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Core.Extensions.Constants;
using SimplCommerce.Infrastructure;

namespace SimplCommerce.Module.Core.Controllers
{
    [Authorize(Policy = Policy.CanManageUser)]
    [Route("api/roles")]
    public class RoleApiController : Controller
    {
        private readonly IRepository<Role> _roleRepository;

        public RoleApiController(IRepository<Role> roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IActionResult> Get()
        {
            var roles = await _roleRepository
                .Query()
                .WhereIf(!User.IsInRole(RoleName.Admin), role => role.Name != RoleName.Admin && role.Name != RoleName.SuperAdmin && role.Name != RoleName.Vendor)
                .Select(x => new
                {
                    x.Id,
                    x.Name
                }).ToListAsync();

            return Json(roles);
        }
    }
}
