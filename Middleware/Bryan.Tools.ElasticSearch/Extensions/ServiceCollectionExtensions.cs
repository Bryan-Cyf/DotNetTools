using System;
using System.Linq;
using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nest;
using Tools.Dependency;
using Tools.Elastic;

public static class ServiceCollectionExtensions
{
    public static void AddElastic(this IServiceCollection services, string sectionName = ElasticOptions.SectionName, Action<ElasticOptions> configure = null)
    {
        using ServiceProvider provider = services.BuildServiceProvider();
        IConfigurationSection section = (provider.GetRequiredService<IConfiguration>() ?? throw new ArgumentNullException("IConfiguration")).GetSection(sectionName);
        if (!section.Exists())
        {
            throw new Exception($"Config file not exist {sectionName} section.");
        }
        ElasticOptions option = section.Get<ElasticOptions>();
        if (option == null)
        {
            throw new Exception($"Get {sectionName} option from config file failed.");
        }

        //注入ES配置
        services.AddOptions<ElasticOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        services.PostConfigure<ElasticOptions>(x =>
        {
            configure?.Invoke(x);
        });

        services.AddSingleton<IElasticClient>(option.CreateClient());

        services.AddSingleton(typeof(IElasticRepository<>), typeof(ElasticRepository<>));

        var types = DiHelper.GetTypes(typeof(IElasticRepository));
        if (types != null && types.Any())
        {
            foreach (var type in types)
            {
                services.TryAddSingleton(type);
            }
        }
    }
}
