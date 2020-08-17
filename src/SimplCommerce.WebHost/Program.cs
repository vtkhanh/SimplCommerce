using System;
using System.IO;
using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.WebHost.Extensions;

namespace SimplCommerce.WebHost
{
    public class Program
    {
        public static void Main(string[] args) =>
            CreateHostBuilder(args)
                .ConfigureLogging(SetupLogging)
                .UseSerilog()
                .Build()
                .Run();

        // For EF to instantiate DbContext object. "BuildWebHost" is a convention!
        private static IHost BuildWebHost(string[] args) => CreateHostBuilder(args).Build();

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .ConfigureAppConfiguration(SetupConfiguration);

        private static void SetupLogging(HostBuilderContext context, ILoggingBuilder loggingBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        private static void SetupConfiguration(HostBuilderContext context, IConfigurationBuilder configBuilder)
        {
            configBuilder
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecretsIf(context.HostingEnvironment.IsDevelopment());
            
            var config = configBuilder.Build();
            configBuilder.AddEntityFrameworkConfig(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"))
            );

        }

    }
}
