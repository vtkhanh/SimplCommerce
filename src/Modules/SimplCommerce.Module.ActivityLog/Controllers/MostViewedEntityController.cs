using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimplCommerce.Module.ActivityLog.Data;
using SimplCommerce.Module.ActivityLog.Models;

namespace SimplCommerce.Module.ActivityLog.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/activitylog")]
    public class MostViewedEntityController : Controller
    {
        private readonly IActivityTypeRepository _activityTypeRepository;
        private readonly ILogger<MostViewedEntityController> _logger;

        public MostViewedEntityController(ILogger<MostViewedEntityController> logger, IActivityTypeRepository activityTypeRepository)
        {
            _logger = logger;
            _activityTypeRepository = activityTypeRepository;
        }

        [HttpGet("most-viewed-entities/{entityTypeId}")]
        public async Task<IList<MostViewEntityDto>> GetMostViewedEntities(long entityTypeId)
        {
            try
            {
                var result = await _activityTypeRepository
                    .List()
                    .Where(x => x.EntityTypeId == entityTypeId)
                    .Take(10)
                    .ToListAsync();

                return result;
            }
            catch (System.Exception exception)
            {
                _logger.LogError(exception, "Error when querying most viewed entities");
                throw;
            }
        }
    }
}
