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

namespace SimplCommerce.Module.Core
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<SignInManager<User>, SimplSignInManager<User>>();
            services.AddScoped<IWorkContext, WorkContext>();
            services.AddScoped<ISmsSender, SmsSender>();
            services.AddAutoMapper();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
        }
    }
}
