using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace SimplCommerce.Infrastructure
{
    public interface IModuleInitializer
    {
        void ConfigureServices(IServiceCollection serviceCollection, IConfiguration configuration);

        void Configure(IApplicationBuilder app, IWebHostEnvironment env);
    }
}
