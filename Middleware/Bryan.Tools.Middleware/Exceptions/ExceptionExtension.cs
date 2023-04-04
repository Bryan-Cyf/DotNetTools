using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using Tools.Middleware.Exceptions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 为了确保应用程序的稳定性和安全性，同时使用 ExceptionMiddleware 和 ExceptionFilter 进行全局异常处理
    /// ExceptionMiddleware 主要负责拦截请求并处理异常
    /// ExceptionFilter 则负责捕获并处理控制器方法中的异常
    /// 这样可以分离异常处理逻辑，提高代码的可读性和可维护性，并确保应用程序的性能和稳定性
    /// </summary>
    public static class ExceptionExtension
    {
        public static IServiceCollection AddException(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(typeof(ExceptionFilter));
            });
            services.AddBuffering();
            return services.AddSingleton<IStartupFilter, ExceptionStartupFilter>();
        }
    }

    internal class ExceptionStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.UseMiddleware<ExceptionMiddleware>();
                next(app);
            };
        }
    }
}
