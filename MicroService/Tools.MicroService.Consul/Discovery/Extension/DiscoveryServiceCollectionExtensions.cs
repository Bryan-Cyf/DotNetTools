using Microsoft.Extensions.DependencyInjection;
using System;
using Tools.MicroService.Consul;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///  服务发现IOC容器扩展
    /// </summary>
    public static class DiscoveryServiceCollectionExtensions
    {

        public static IServiceCollection AddServiceDiscovery(this IServiceCollection services, Action<ConsulDiscoveryOptions> options)
        {
            // 2、注册到IOC容器
            services.Configure<ConsulDiscoveryOptions>(options);

            // 3、注册consul服务发现
            services.AddSingleton<IServiceDiscovery, ConsulServiceDiscovery>();

            return services;
        }

    }
}
