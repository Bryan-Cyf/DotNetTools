using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using Tools.Middleware.Monitor;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TimeMonitorExtension
    {
        public static IServiceCollection AddTimeMonitor(this IServiceCollection services, Action<TimeMonitorOption> configure = null)
        {
            services.AddOptions<TimeMonitorOption>();

            services.PostConfigure<TimeMonitorOption>(x =>
            {
                configure?.Invoke(x);
            });

            services.AddBuffering();

            return services.AddSingleton<IStartupFilter, TimeMonitorStartupFilter>();
        }
    }

    internal class TimeMonitorStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.UseMiddleware<TimeMonitorMiddleware>();
                next(app);
            };
        }
    }
}
