using System.Linq;
using SimplCommerce.Module.ActivityLog.Models;
using SimplCommerce.Module.Core.Data;
using SimplCommerce.Module.Core.Models;

namespace SimplCommerce.Module.ActivityLog.Data
{
    public class ActivityRepository : Repository<Activity>, IActivityTypeRepository
    {
        private const int MostViewActivityTypeId = 1;

        public ActivityRepository(SimplDbContext context) : base(context)
        {
        }

        public IQueryable<MostViewEntityDto> List()
        {
            var result = Query()
                .Join(Context.Set<Entity>(), 
                        a => new { a.EntityId, a.EntityTypeId }, 
                        e => new { e.EntityId, e.EntityTypeId}, 
                        (a, e) => new { 
                            EntityId = a.EntityId,
                            EntityTypeId = a.EntityTypeId,
                            Name = e.Name,
                            Slug = e.Slug
                        })
                .GroupBy(i => new { i.EntityId, i.EntityTypeId, i.Name, i.Slug })
                .OrderByDescending(g => g.Count())
                .Select(g => new MostViewEntityDto() {
                    EntityId = g.Key.EntityId,
                    EntityTypeId = g.Key.EntityTypeId,
                    Name = g.Key.Name,
                    Slug = g.Key.Slug,
                    ViewedCount = g.Count()
                });

            return result;
        }
    }
}
