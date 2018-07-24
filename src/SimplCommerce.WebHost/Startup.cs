﻿using System;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using SimplCommerce.Infrastructure;
using SimplCommerce.Infrastructure.Web;
using SimplCommerce.Module.Localization;
using SimplCommerce.WebHost.Extensions;

namespace SimplCommerce.WebHost
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            GlobalConfiguration.WebRootPath = _hostingEnvironment.WebRootPath;
            GlobalConfiguration.ContentRootPath = _hostingEnvironment.ContentRootPath;

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

            return services.Build(_configuration, _hostingEnvironment);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStatusCodePagesWithReExecute("/Home/ErrorWithCode/{0}");

            app.UseCustomizedRequestLocalization();
            app.UseCustomizedStaticFiles(env);
            app.UseCustomizedIdentity();
            app.UseCustomizedMvc();

            app.RunModuleConfigures(env);
        }
    }
}
