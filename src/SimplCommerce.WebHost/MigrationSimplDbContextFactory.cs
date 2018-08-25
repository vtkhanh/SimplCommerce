using System;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimplCommerce.Module.Core.Data;
using SimplCommerce.WebHost.Extensions;
using Microsoft.EntityFrameworkCore.Design;

namespace SimplCommerce.WebHost
{
    public class MigrationSimplDbContextFactory : IDesignTimeDbContextFactory<SimplDbContext>
    {
        public SimplDbContext CreateDbContext(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var contentRootPath = Directory.GetCurrentDirectory();

            var configuration = new ConfigurationBuilder()
                            .SetBasePath(contentRootPath)
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{environmentName}.json", true)
                            .AddUserSecretsIf(environmentName == "Development")
                            .AddEnvironmentVariables()
                            .Build();

            IServiceCollection services = new ServiceCollection();
            services.AddCustomizedDataStore(configuration);

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider.GetRequiredService<SimplDbContext>();
        }
    }
}
