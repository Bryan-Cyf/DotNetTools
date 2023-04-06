using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using Tools.Hangfire;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration configuration, Action<HangfireOptions> configure = null, string sectionName = null)
        {
            sectionName ??= HangfireOptions.SectionName;

            services.AddOptions<HangfireOptions>()
                .Bind(configuration.GetSection(sectionName))
                .ValidateDataAnnotations();

            services.PostConfigure<HangfireOptions>(x =>
            {
                configure?.Invoke(x);
            });

            var options = configuration.GetSection(HangfireOptions.SectionName).Get<HangfireOptions>() ?? new HangfireOptions();
            configure?.Invoke(options);

            if (options.IsOpenServer)
            {
                services.AddHangfire(x =>
                {
                    x.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                     .UseSimpleAssemblyNameTypeSerializer()
                     .UseRecommendedSerializerSettings()
                     .UseStorage(options);
                });

                services.AddHangfireServer(x =>
                {
                    x.ServerName = options.ServerName ?? HangfireOptions.DefaultUserName;
                    x.WorkerCount = options.WorkCount > 0 ? options.WorkCount : Math.Min(Environment.ProcessorCount * 5, 20);
                    x.SchedulePollingInterval = TimeSpan.FromSeconds(options.ScheduleInterval);
                    x.Queues = options.Queues ?? new string[] { "default" };
                });

                services.AddSingleton<IStartupFilter, HangfireServerStartupFilter>();
                //自动注册定时任务
                services.AddSingleton<IHostedService, HangfireRecurringHostService>();

                var types = DiHelper.GetTypes(typeof(IHangfireReccuring));
                if (types != null && types.Any())
                {
                    foreach (var type in types)
                    {
                        services.TryAddSingleton(typeof(IHangfireReccuring), type);
                    }
                }
            }

            return services;
        }
    }
}
