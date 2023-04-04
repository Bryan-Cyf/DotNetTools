using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tools.MicroService.Consul.HealthCheck.Filter;

namespace Tools.MicroService.Consul.HealthCheck.Extension
{
    public static class HealthCheckServiceCollectionExtensions
    {

        /// <summary>
        /// 健康检查服务注册
        /// </summary>
        public static IServiceCollection AddHealthCheckServer(this IServiceCollection services)
        {
            return services.AddHealthCheckServer(x => { });
        }

        /// <summary>
        /// 健康检查服务注册
        /// </summary>
        public static IServiceCollection AddHealthCheckServer(this IServiceCollection services, Action<HealthCheckOptions> options)
        {
            services.AddHealthChecks();
            services.Configure<HealthCheckOptions>(options);
            services.AddSingleton<IStartupFilter, HealthCheckServerStartupFilter>();
            return services;
        }

    }
}
