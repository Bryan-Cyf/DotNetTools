using Microsoft.Extensions.Configuration;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Polly;
using Ocelot.Provider.Consul;
using Microsoft.AspNetCore.Hosting;
using Tools.MicroService.Ocelot;

/// <summary>
/// Ocelot扩展方法
/// </summary>
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddOcelotServer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOcelot(configuration)
                    .AddPolly()
                    .AddConsul();

            services.AddSingleton<IStartupFilter, OcelotServerStartupFilter>();

            return services;
        }
    }
}

