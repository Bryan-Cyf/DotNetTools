using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tools.MicroService.Consul;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RegistryServiceCollectionExtensions
    {
        /// <summary>
        /// Consul服务注册
        /// </summary>
        public static IServiceCollection AddConsulRegistry(this IServiceCollection services, Action<ConsulRegistryOptions> options)
        {
            return services.AddConsulRegistry(null, options);
        }

        public static IServiceCollection AddConsulRegistry(this IServiceCollection services, IConfiguration configuration, Action<ConsulRegistryOptions> options = null, string sectionName = null)
        {
            if (configuration == null)
            {
                services.Configure<ConsulRegistryOptions>(options);
            }
            else
            {
                sectionName ??= ConsulRegistryOptions.SectionName;

                services.AddOptions<ConsulRegistryOptions>()
                    .Bind(configuration.GetSection(sectionName));

                services.PostConfigure<ConsulRegistryOptions>(x =>
                {
                    options?.Invoke(x);
                });
            }

            services.AddSingleton<ConsulServiceRegistry>();

            services.AddHostedService<ServiceRegistryIHostedService>();

            return services;
        }
    }
}
