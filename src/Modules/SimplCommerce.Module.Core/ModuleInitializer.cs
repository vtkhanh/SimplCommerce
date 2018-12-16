using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using SimplCommerce.Infrastructure;
using Microsoft.AspNetCore.Identity;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.Module.Core.Services;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using SimplCommerce.Module.Core.Data;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Core.Extensions.Constants;

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

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
        }

        private void ConfigurePolicies(IServiceCollection services)
        {
            services.AddAuthorization(option =>
            {
                option.AddPolicy(Policy.CanEditProduct, policyBuilder => policyBuilder.RequireRole(RoleName.Admin, RoleName.Vendor));
                option.AddPolicy(Policy.CanAccessDashboard, policyBuilder => policyBuilder.RequireRole(RoleName.Admin, RoleName.Seller, RoleName.Vendor));
                option.AddPolicy(Policy.CanManageOrder, policyBuilder => policyBuilder.RequireRole(RoleName.Admin, RoleName.Seller));
            });
        }

    }
}
