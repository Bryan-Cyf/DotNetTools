using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;

namespace Tools.Swagger
{
    internal static class SwaggerExtension
    {
        /// <summary>
        /// 路径 docz/index.html
        /// </summary>
        /// <param name="pathBase">不需要带/</param>
        internal static IApplicationBuilder UseSwaggerz(this IApplicationBuilder app)
        {
            var option = app.ApplicationServices.GetRequiredService<IOptions<SwaggerOptions>>().Value;
            if (option.IsOpen)
            {
                option.Infos ??= new List<OpenApiInfo>();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    option.Infos.ForEach(info =>
                    {
                        c.SwaggerEndpoint($"/swagger/{info.Version}/swagger.json", info.Title);
                    });
                    c.RoutePrefix = option.RoutePrefix;
                });
            }
            return app;
        }
    }
}
