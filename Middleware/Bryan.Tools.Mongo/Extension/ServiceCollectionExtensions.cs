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
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration, Action<MongoOptions> configure = null, string sectionName = null)
    {
        sectionName ??= MongoOptions.SectionName;

        //注入mongo配置
        services.AddOptions<MongoOptions>()
            .Bind(configuration.GetSection(sectionName))
            .ValidateDataAnnotations();

        services.PostConfigure<MongoOptions>(x =>
        {
            //配置BsonChunkPool 
            if (x.BsonChunkPool != null)
            {
                BsonChunkPool.Default = x.BsonChunkPool;
            }
            configure?.Invoke(x);
        });

        var options = configuration.GetSection(MongoOptions.SectionName).Get<MongoOptions>();

        //注入mongo客户端
        services.TryAddSingleton<IMongoClient>(x => new MongoClient(options.Connection));

        //注入mongo默认数据库
        services.TryAddSingleton<IMongoDatabase>(x => x.GetRequiredService<IMongoClient>().GetDatabase(options.DatabaseName));

        //自动创建和删除索引
        services.AddSingleton<IHostedService, MongoIndexHostService>();

        //注入仓储实现
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
