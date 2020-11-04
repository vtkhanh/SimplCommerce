using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimplCommerce.Infrastructure;
using SimplCommerce.Infrastructure.Web.ModelBinders;
using SimplCommerce.Module.Core.Data;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Models;

namespace SimplCommerce.WebHost.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection LoadModuleInitializers(this IServiceCollection services)
        {
            foreach (var module in GlobalConfiguration.Modules)
            {
                var moduleInitializerType = 
                    module.Assembly.GetTypes().FirstOrDefault(x => typeof(IModuleInitializer).IsAssignableFrom(x));

                if ((moduleInitializerType != null) && (moduleInitializerType != typeof(IModuleInitializer)))
                {
                    services.AddSingleton(typeof(IModuleInitializer), moduleInitializerType);
                }
            }

            return services;
        }

        public static IServiceCollection RunModuleConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var sp = services.BuildServiceProvider();
            var moduleInitializers = sp.GetServices<IModuleInitializer>();
            foreach (var moduleInitializer in moduleInitializers)
            {
                moduleInitializer.ConfigureServices(services, configuration);
            }

            return services;
        }

        public static IServiceCollection AddCustomizedMvc(this IServiceCollection services, IList<ModuleInfo> modules)
        {
            // Class used to map a slug to controller/action
            services.AddScoped<UrlSlugRouteTransformer>();
            
            var mvcBuilder = services
                .AddMvc(options =>
                {
                    options.ModelBinderProviders.Insert(0, new InvariantDecimalModelBinderProvider());
                })
                .AddRazorRuntimeCompilation()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

            foreach (var module in modules)
            {
                mvcBuilder.AddApplicationPart(module.Assembly);
            }

            return services;
        }

        public static IServiceCollection AddCustomizedIdentity(this IServiceCollection services)
        {
            services
                .AddIdentity<User, Role>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 4;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequiredUniqueChars = 0;
                })
                .AddRoleStore<SimplRoleStore>()
                .AddUserStore<SimplUserStore>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o => o.LoginPath = new PathString("/login"));

            services.ConfigureApplicationCookie(x => x.LoginPath = new PathString("/login"));
            return services;
        }

        public static IServiceCollection AddCustomizedDataStore(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextPool<SimplDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("SimplCommerce.WebHost")));
            return services;
        }
    }
}
