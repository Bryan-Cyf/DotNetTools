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
    public static void AddElastic(this IServiceCollection services, IConfiguration configuration, Action<ElasticOptions> configure = null, string sectionName = null)
    {
        sectionName ??= ElasticOptions.SectionName;

        //注入ES配置
        services.AddOptions<ElasticOptions>()
            .Bind(configuration.GetSection(sectionName))
            .ValidateDataAnnotations();

        services.PostConfigure<ElasticOptions>(x =>
        {
            configure?.Invoke(x);
        });

        ElasticClient client = configuration.CreateClient();

        services.AddSingleton<IElasticClient>(client);

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
