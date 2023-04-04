using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;


namespace Tools.MicroService.Consul.HealthCheck.Filter
{
    public class HealthCheckServerStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                var options = app.ApplicationServices.GetService<IOptions<HealthCheckOptions>>().Value;
                app.UseHealthChecks(options.HealthCheckAddress);
                next(app);
            };
        }
    }
}
