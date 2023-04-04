using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using Tools.Swagger;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 生成带Bearer授权的接口文档
    /// </summary>
    public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration, Action<SwaggerOptions> configure = null, Action<SwaggerGenOptions> confirureSwaggerGenOptions = null)
    {
        services.AddOptions<SwaggerOptions>()
            .Bind(configuration.GetSection(SwaggerOptions.SectionName))
            .ValidateDataAnnotations();

        services.PostConfigure<SwaggerOptions>(x =>
        {
            configure?.Invoke(x);
        });

        var options = configuration.GetSection(SwaggerOptions.SectionName).Get<SwaggerOptions>() ?? new SwaggerOptions();

        if (options.IsOpen)
        {
            services.AddSingleton<IStartupFilter, SwaggerStartupFilter>();

            services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(type => type.FullName);
                c.OrderActionsBy(x => x.RelativePath);
                //添加注释文档
                if (options.IncludeXmlComments == null) options.IncludeXmlComments = new List<string>();
                //if (!options.IncludeXmlComments.Contains(Assembly.GetEntryAssembly().GetName().Name + ".xml")) options.IncludeXmlComments.Add(Assembly.GetEntryAssembly().GetName().Name + ".xml");
                foreach (var item in options.IncludeXmlComments)
                {
                    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, item), true);
                }

                c.OrderActionsBy(x => x.RelativePath);
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "接口文档", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "请输入Token(需要带有Bearer)",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                });

                //Json Token认证方式，此方式为全局添加
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme {
                        Reference = new OpenApiReference(){
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        } },
                        Array.Empty<string>()
                     }
                });

                confirureSwaggerGenOptions?.Invoke(c);
            });
        }

        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services, Action<SwaggerOptions> configure = null, Action<SwaggerGenOptions> confirureSwaggerGenOptions = null)
    {
        services.AddOptions<SwaggerOptions>();

        services.PostConfigure<SwaggerOptions>(x =>
        {
            configure?.Invoke(x);
        });

        var options = new SwaggerOptions();
        configure?.Invoke(options);

        if (options.IsOpen)
        {
            services.AddSingleton<IStartupFilter, SwaggerStartupFilter>();

            services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(type => type.FullName);
                c.OrderActionsBy(x => x.RelativePath);
                //添加注释文档
                if (options.IncludeXmlComments == null) options.IncludeXmlComments = new List<string>();
                foreach (var item in options.IncludeXmlComments)
                {
                    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, item), true);
                }

                c.OrderActionsBy(x => x.RelativePath);
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "接口文档", Version = "v1" });

                confirureSwaggerGenOptions?.Invoke(c);
            });
        }

        return services;
    }

}
