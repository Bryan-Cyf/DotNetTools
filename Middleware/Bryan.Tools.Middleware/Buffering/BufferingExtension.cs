using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BufferingExtension
    {
        public static IServiceCollection AddBuffering(this IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            return services.AddSingleton<IStartupFilter, BufferingStartupFilter>();
        }
    }

    internal class BufferingStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.Use(next => context =>
                {
                    context.Request.EnableBuffering();
                    return next(context);
                });
                next(app);
            };
        }
    }

}
