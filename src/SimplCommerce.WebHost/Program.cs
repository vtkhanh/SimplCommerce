using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.WebHost.Extensions;

namespace SimplCommerce.WebHost
{
    public class Program
    {
        public static void Main(string[] args) =>
            CreateWebHostBuilder(args)
                .ConfigureLogging(SetupLogging)
                .UseSerilog()
                .UseApplicationInsights()
                .Build()
                .Run();

        // For EF to instantiate DbContext object. "BuildWebHost" is a convention!
        private static IWebHost BuildWebHost(string[] args) =>
            CreateWebHostBuilder(args).Build();

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            Microsoft.AspNetCore.WebHost
                .CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration(SetupConfiguration);

        private static void SetupLogging(WebHostBuilderContext context, ILoggingBuilder loggingBuilder)
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

        private static void SetupConfiguration(WebHostBuilderContext context, IConfigurationBuilder configBuilder) =>
            configBuilder
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecretsIf(context.HostingEnvironment.IsDevelopment());

    }
}