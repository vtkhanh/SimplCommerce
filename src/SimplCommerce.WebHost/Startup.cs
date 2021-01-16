using System;
using Autofac;
using Autofac.Features.Variance;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SimplCommerce.Infrastructure;
using SimplCommerce.Infrastructure.Web;
using SimplCommerce.Module.Core.Data;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Localization;
using SimplCommerce.WebHost.Extensions;

namespace SimplCommerce.WebHost
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            GlobalConfiguration.WebRootPath = _environment.WebRootPath;
            GlobalConfiguration.ContentRootPath = _environment.ContentRootPath;

            // Add functionality to inject IOptions<T>
            services.AddOptions();

            services.LoadModuleInitializers().RunModuleConfigureServices(_configuration);

            services.AddCustomizedDataStore(_configuration);
            services.AddCustomizedIdentity();

            services.AddSingleton<IStringLocalizerFactory, EfStringLocalizerFactory>();
            services.AddSingleton<IRazorViewRenderer, RazorViewRenderer>();

            services.AddCloudscribePagination();

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ModuleViewLocationExpander());
            });

            services.AddCustomizedMvc(GlobalConfiguration.Modules);

            services.AddAutoMapper();

            services.AddMediatR();

            services.AddHttpContextAccessor();

            services.AddApplicationInsightsTelemetry();
        }

        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterSource(new ContravariantRegistrationSource());

            foreach (var module in GlobalConfiguration.Modules)
            {
                builder.RegisterAssemblyTypes(module.Assembly)
                    .Where(t => t.Name.EndsWith("Repository"))
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();
                builder.RegisterAssemblyTypes(module.Assembly)
                    .Where(t => t.Name.EndsWith("Service"))
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();
                builder.RegisterAssemblyTypes(module.Assembly)
                    .Where(t => t.Name.EndsWith("ServiceProvider"))
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();
                builder.RegisterAssemblyTypes(module.Assembly)
                    .Where(t => t.Name.EndsWith("Handler"))
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SimplDbContext dataContext)
        {
            dataContext.Database.Migrate();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStatusCodePagesWithReExecute("/Home/ErrorWithCode/{0}");

            app.UseCustomizedRequestLocalization();
            app.UseCustomizedStaticFiles(env);
            
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => 
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapDynamicControllerRoute<UrlSlugRouteTransformer>("{**slug}");
            });

            app.RunModuleConfigures(env);
        }
    }
}
