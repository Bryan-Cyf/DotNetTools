using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tools.Mongo;
using Tools.Dependency;
using Microsoft.Extensions.Hosting;

/// <summary>
/// mongoHelper扩展方法
/// </summary>
public static class MongoExtensions
{
    public static List<Type> _mongoRepositoryTypes { get; } = new List<Type>();
    /// <summary>
    /// 注入mongo的配置,客户端,默认数据库,仓储类
    /// </summary>
    /// <param name="services">服务</param>
    /// <param name="configuration">配置</param>
    /// <param name="configure"></param>
    public static IServiceCollection AddMongoDb(this IServiceCollection services, string sectionName = MongoOptions.SectionName, Action<MongoOptions> configure = null)
    {
        using ServiceProvider provider = services.BuildServiceProvider();
        IConfigurationSection section = (provider.GetRequiredService<IConfiguration>() ?? throw new ArgumentNullException("IConfiguration")).GetSection(sectionName);
        if (!section.Exists())
        {
            throw new Exception($"Config file not exist {sectionName} section.");
        }
        MongoOptions option = section.Get<MongoOptions>();
        if (option == null)
        {
            throw new Exception($"Get {sectionName} option from config file failed.");
        }

        services.AddOptions<MongoOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        services.PostConfigure<MongoOptions>(x =>
        {
            if (x.BsonChunkPool != null)
            {
                BsonChunkPool.Default = x.BsonChunkPool;
            }
            configure?.Invoke(x);
        });


        services.TryAddSingleton<IMongoClient>(x => new MongoClient(option.Connection));

        //注入mongo默认数据库
        services.TryAddSingleton<IMongoDatabase>(x => x.GetRequiredService<IMongoClient>().GetDatabase(option.DatabaseName));

        //自动创建和删除索引
        services.AddSingleton<IHostedService, MongoIndexHostService>();

        services.TryAddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));

        var types = DiHelper.GetTypes(typeof(IMongoRepository));
        if (types?.Count() > 0)
        {
            foreach (var item in types)
            {
                if (item.Name != (typeof(MongoRepository<>)).Name)
                {
                    _mongoRepositoryTypes.Add(item);
                    services.TryAddSingleton(item);
                }
            }
            Console.WriteLine($"mongo仓储--{string.Join(",", types.Select(x => x.Name))}");
        }

        //增加DateTimeOffset序列化
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSupportingBsonDateTimeSerializer());

        //指定Dictionary存储方式
        ConventionRegistry.Register("DictionaryRepresentationConvention", new ConventionPack { new DictionaryRepresentationConvention(DictionaryRepresentation.ArrayOfArrays) }, _ => true);

        return services;
    }
}
