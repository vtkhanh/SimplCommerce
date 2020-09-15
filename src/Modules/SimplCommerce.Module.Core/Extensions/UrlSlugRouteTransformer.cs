using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Core.Models;

namespace SimplCommerce.Module.Core.Extensions
{
    public class UrlSlugRouteTransformer : DynamicRouteValueTransformer
    {
        private readonly IRepository<Entity> _repository;

        public UrlSlugRouteTransformer(IRepository<Entity> repository)
        {
            _repository = repository;
        }

        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            var slug = values["slug"] as string;
            if (!string.IsNullOrEmpty(slug) && slug[0] == '/')
            {
                // Trim the leading slash
                slug = slug.Substring(1);
            }

            var urlSlug = await _repository.Query().Include(x => x.EntityType).FirstOrDefaultAsync(x => x.Slug == slug);

            if (urlSlug is null) {
                return null;
            }

            return new RouteValueDictionary()
            {
                { "controller", urlSlug.EntityType.RoutingController },
                { "action", urlSlug.EntityType.RoutingAction },
                { "id", urlSlug.EntityId }
            };
        }
    }
}