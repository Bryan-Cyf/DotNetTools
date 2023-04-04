using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using MessagePack.Resolvers;
using Tools.Cache;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 注册缓存服务,默认启用内存和csredis,使用MessagePack序列化(msgpack)
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration, Action<CacheOptions> configure = null)
    {
        //注入配置
        services.AddOptions<CacheOptions>()
            .Bind(configuration.GetSection(CacheOptions.SectionName))
            .ValidateDataAnnotations();

        services.PostConfigure<CacheOptions>(x =>
        {
            configure?.Invoke(x);
        });

        services.AddSingleton<IEasyMemoryCache, MemoryEasyManager>();
        services.AddSingleton<IEasyRedisCache, RedisEasyManager>();
        services.AddSingleton<IEasyHybirdCache, HybirdEasyManager>();

        var options = configuration.GetSection(CacheOptions.SectionName).Get<CacheOptions>();

        services.AddEasyCaching(x =>
        {
            x.UseInMemory(x =>
            {
                x.SerializerName = CacheConst.SerializerName;
                //数据量大小限制
                x.DBConfig.SizeLimit = options?.MemorySizeLimit;
            }, CacheConst.Memmory);

            if (options != null && options.IsUseRedis && options.Connections != null && options.Connections.Any())
            {
                x.UseCSRedis(x =>
                {
                    x.SerializerName = CacheConst.SerializerName;
                    // 互斥锁的存活时间。默认值:5000
                    x.LockMs = 5000;
                    // 预防缓存在同一时间全部失效，可以为每个key的过期时间添加一个随机的秒数。默认值:120秒
                    x.MaxRdSecond = 0;
                    // 没有获取到互斥锁时的休眠时间。默认值:300毫秒
                    x.SleepMs = 300;
                    x.DBConfig.ConnectionStrings = options.Connections;
                }, CacheConst.CsRedis);

                x.UseHybrid(config =>
                {
                    config.EnableLogging = false;
                    config.TopicName = CacheConst.TopicName;
                    config.LocalCacheProviderName = CacheConst.Memmory;
                    config.DistributedCacheProviderName = CacheConst.CsRedis;
                }, CacheConst.Hybird);

                x.WithCSRedisBus(x =>
                {
                    x.ConnectionStrings = options.Connections;
                });
            }

            //解决时间非本地时间的问题
            x.WithMessagePack(x =>
            {
                x.EnableCustomResolver = true;
                x.CustomResolvers = CompositeResolver.Create(NativeDateTimeResolver.Instance, ContractlessStandardResolver.Instance);
            }, CacheConst.SerializerName);

        });
        if (options != null && options.IsUseRedis && options.Connections != null && options.Connections.Any())
        {
            RedisHelper.Initialization(new CSRedis.CSRedisClient(options.Connections.First()));
        }
        return services;
    }
}
