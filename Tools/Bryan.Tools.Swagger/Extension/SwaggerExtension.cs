using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
            var options = app.ApplicationServices.GetRequiredService<IOptions<SwaggerOptions>>().Value;

            app.UseSwagger(x =>
            {
                x.RouteTemplate = $"{options.RoutePrefix}/{{documentName}}/swagger.json";
                options.ConfigareSwaggerOptions?.Invoke(x);
            });

            app.UseSwaggerUI(c =>
            {
                var url = string.IsNullOrEmpty(options.PathBase) ? $"/{options.RoutePrefix}/v1/swagger.json" : $"/{options.PathBase}/{options.RoutePrefix}/v1/swagger.json";
                c.SwaggerEndpoint(url, "api v1");
                c.RoutePrefix = $"{options.RoutePrefix}";

                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);

                options.ConfigareSwaggerUIOptions?.Invoke(c);
            });
            return app;
        }
    }
}
