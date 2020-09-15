using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimplCommerce.Infrastructure;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Core.Data;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Extensions.Constants;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Core.Services;

namespace SimplCommerce.Module.Core
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<SignInManager<User>, SimplSignInManager<User>>();
            services.AddScoped<IWorkContext, WorkContext>();
            services.AddScoped<ISmsSender, SmsSender>();

            // Repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IRepositoryWithTypedId<,>), typeof(RepositoryWithTypedId<,>));


            ConfigurePolicies(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }

        private void ConfigurePolicies(IServiceCollection services)
        {
            services.AddAuthorization(option =>
            {
                option.AddPolicy(Policy.CanEditProduct, policyBuilder => policyBuilder.RequireRole(RoleName.Admin, RoleName.Vendor));
                option.AddPolicy(Policy.CanAccessDashboard, policyBuilder => policyBuilder.RequireRole(RoleName.Admin, RoleName.Seller, RoleName.Vendor));
                option.AddPolicy(Policy.CanManageOrder, policyBuilder => policyBuilder.RequireRole(RoleName.Admin, RoleName.Seller));
                option.AddPolicy(Policy.CanManageUser, policyBuilder => policyBuilder.RequireRole(RoleName.Admin, RoleName.Seller));
            });
        }

    }
}
