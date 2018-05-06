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
        public async Task<IEnumerable<MostViewEntityDto>> GetMostViewedEntities(long entityTypeId)
        {
            try
            {
                const int MostViewedCount = 10;
                var list = await _activityTypeRepository
                    .List()
                    .ToListAsync();
                var result = list
                    .Where(x => x.EntityTypeId == entityTypeId)
                    .OrderByDescending(x => x.ViewedCount)
                    .Take(MostViewedCount);

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
