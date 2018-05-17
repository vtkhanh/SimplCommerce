using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using SimplCommerce.Infrastructure;
using SimplCommerce.Module.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace SimplCommerce.Module.StorageAzureBlob
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IMediaService, AzureBlobMediaStorage>();

            services.Configure<AzureStorageConfig>(configuration.GetSection("AzureStorageConfig"));
            services.AddTransient<AzureStorageConfig>((opt) => opt.GetService<IOptionsSnapshot<AzureStorageConfig>>().Value);
        }
    }
}
