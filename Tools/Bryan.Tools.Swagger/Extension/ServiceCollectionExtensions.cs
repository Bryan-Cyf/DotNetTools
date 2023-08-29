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
    public static IServiceCollection AddSwagger(this IServiceCollection services, string sectionName = SwaggerOptions.SectionName, Action<SwaggerOptions> configure = null, Action<SwaggerGenOptions> confirureSwaggerGenOptions = null)
    {
        using ServiceProvider provider = services.BuildServiceProvider();
        IConfigurationSection section = (provider.GetRequiredService<IConfiguration>() ?? throw new ArgumentNullException("IConfiguration")).GetSection(sectionName);
        if (!section.Exists())
        {
            throw new Exception("Config file not exist '" + sectionName + "' section.");
        }
        SwaggerOptions option = section.Get<SwaggerOptions>();
        if (option == null)
        {
            throw new Exception($"Get Swagger option from config file failed.");
        }

        option.Infos ??= new List<OpenApiInfo>();

        option.IncludeXmlComments ??= new List<string>();

        services.AddOptions<SwaggerOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        services.PostConfigure<SwaggerOptions>(x =>
        {
            configure?.Invoke(x);
        });

        services.AddSwaggerGen(c =>
        {
            option.Infos.ForEach(info =>
            {
                c.SwaggerDoc(info.Version, info);
            });

            option.IncludeXmlComments.ForEach(xml =>
            {
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xml), true);//显示控制器xml注释内容
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Description = "JWT授权token前面需要加上字段Bearer与一个空格,如Bearer token",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });

        services.AddSingleton<IStartupFilter, SwaggerStartupFilter>();

        return services;
    }

}
