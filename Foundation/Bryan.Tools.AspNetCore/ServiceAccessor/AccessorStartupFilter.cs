using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Tools.AspNetCore
{
    internal class AccessorStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
                HttpServiceAccessor.Configure(httpContextAccessor);
                next(app);
            };
        }
    }
}
