using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace SimplCommerce.WebHost.Extensions
{
    public static class ConfigurationBuilderExtensions 
    {
        public static IConfigurationBuilder AddUserSecretsIf(this IConfigurationBuilder configBuilder, bool conditional) =>
            conditional ? configBuilder.AddUserSecrets<Startup>() : configBuilder;
    }
}