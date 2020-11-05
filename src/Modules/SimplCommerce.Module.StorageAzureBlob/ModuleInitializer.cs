using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimplCommerce.Infrastructure;
using SimplCommerce.Module.Core.Services;

namespace SimplCommerce.Module.StorageAzureBlob
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IMediaService, AzureMediaStorageService>();
            services.AddScoped<IOrderFileStorageService, AzureOrderFileStorageService>();

            services.AddScoped((opt) => opt.GetService<IOptionsSnapshot<AzureStorageConfig>>().Value);

            services.Configure<AzureStorageConfig>("AzureOrderFileStorageConfig", configuration.GetSection("AzureOrderFileStorageConfig"));
            services.Configure<AzureStorageConfig>("AzureMediaStorageConfig", configuration.GetSection("AzureMediaStorageConfig"));
        }
    }
}
