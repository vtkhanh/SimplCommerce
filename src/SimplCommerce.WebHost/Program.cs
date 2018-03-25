using System;
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
            CreateWebHostBuilder(args).Build().Run();

        // For EF to instantiate DbContext object. "BuildWebHost" is a convention!
        public static IWebHost BuildWebHost(string[] args) =>
            CreateWebHostBuilder(args).Build();

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            Microsoft.AspNetCore.WebHost
                .CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration(SetupConfiguration)
                .ConfigureLogging(SetupLogging);

        private static void SetupLogging(WebHostBuilderContext context, ILoggingBuilder loggingBuilder) =>
            loggingBuilder
                .AddConfiguration(context.Configuration.GetSection("Logging"))
                .AddConsole()
                .AddDebug();

        private static void SetupConfiguration(WebHostBuilderContext context, IConfigurationBuilder configBuilder) =>
            configBuilder
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecretsIf(context.HostingEnvironment.IsDevelopment());

    }
}
