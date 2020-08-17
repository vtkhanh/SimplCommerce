﻿using SimplCommerce.Infrastructure;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using SimplCommerce.Module.Core.Services;
using Microsoft.Extensions.Configuration;

namespace SimplCommerce.Module.EmailSenderSmtp
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddScoped<IEmailSender, EmailSender>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}
