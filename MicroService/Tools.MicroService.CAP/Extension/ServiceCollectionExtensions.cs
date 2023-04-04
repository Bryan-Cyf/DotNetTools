using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Tools.MicroService.CAP;
using DotNetCore.CAP;
using Microsoft.Extensions.DependencyInjection.Extensions;
using CapOptions = Tools.MicroService.CAP.CapOptions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCapServer(this IServiceCollection services, Action<CapOptions> configure)
        {
            var options = new CapOptions();
            configure.Invoke(options);

            services.AddCap(x =>
            {
                x.UseRabbitMQ(rb =>
                {
                    rb.HostName = options.HostName;
                    rb.UserName = options.UserName;
                    rb.Password = options.Password;
                    rb.Port = options.Port;
                    rb.VirtualHost = options.VirtualHost;
                });

                x.UseMySql(options.Connection);
                x.DefaultGroupName = "default-group-name";
                x.FailedRetryInterval = options.FailedRetryInterval;
                x.FailedRetryCount = options.FailedRetryCount;
                x.UseDashboard();
            });

            var types = DiHelper.GetTypes(typeof(ICapSubscribe));
            if (types?.Count() > 0)
            {
                foreach (var item in types)
                {
                    services.TryAddSingleton(item);
                }
            }
            return services;
        }

        public static IServiceCollection AddCapServer(this IServiceCollection services, IConfiguration configuration, string sectionName = null)
        {
            sectionName ??= CapOptions.SectionName;

            var options = configuration.GetSection(sectionName).Get<CapOptions>() ?? new CapOptions();
            services.AddCapServer(x =>
            {
                x.HostName = options.HostName;
                x.Port = options.Port;
                x.UserName = options.UserName;
                x.Password = options.Password;
                x.VirtualHost = options.VirtualHost;
                x.Connection = options.Connection;
                x.FailedRetryCount = options.FailedRetryCount;
                x.FailedRetryInterval = options.FailedRetryInterval;
            });

            return services;
        }
    }
}

