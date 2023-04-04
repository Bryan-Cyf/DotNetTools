using EasyCaching.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tools.Cache
{
    /// <summary>
    /// EasyCaching缓存服务
    /// </summary>
    public abstract class EasyRedisBaseManager : EasyBaseManager, IEasyRedisCache
    {

        /// <summary>
        /// EasyCaching缓存提供器
        /// </summary>
        public IRedisCachingProvider RedisProvider { get; set; }

        public EasyRedisBaseManager(IEasyCachingProviderFactory factory) : base(factory)
        {
            RedisProvider = factory.GetRedisProvider(Name);
        }

        /// <summary>
        /// 设置键过期时间
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="second">秒数</param>
        /// <returns></returns>
        public async Task<bool> KeyExpireAsync(string cacheKey, int second) => await RedisProvider.KeyExpireAsync(cacheKey, second);

        #region----Hash----

        /// <summary>
        /// Hash 获取指定字段的值
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        public async Task<string> HGetAsync(string cacheKey, string field)
        {
            return await RedisProvider.HGetAsync(cacheKey, field);
        }

        /// <summary>
        /// Hash 获取指定字段的值
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        public async Task<T> HGetAsync<T>(string cacheKey, string field) where T : class
        {
            var result = await RedisProvider.HGetAsync(cacheKey, field);
            return string.IsNullOrWhiteSpace(result) ? default(T) : result.ToObj<T>();
        }

        /// <summary>
        /// Hash 覆盖设置指定键字段值
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="field"></param>
        /// <param name="cacheValue"></param>
        /// <returns></returns>
        public async Task<bool> HSetAsync(string cacheKey, string field, string cacheValue) => await RedisProvider.HSetAsync(cacheKey, field, cacheValue);

        /// <summary>
        /// Hash 自增/自减指定字段的数值
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="field">字段</param>
        /// <param name="val">+增加/-减少数量</param>
        /// <returns></returns>
        public async Task<long> HIncrByAsync(string cacheKey, string field, long val = 1) => await RedisProvider.HIncrByAsync(cacheKey, field, val);

        /// <summary>
        /// Hash 删除一个或多个指定域
        /// O(N)， N为要删除的域的数量
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="fields">删除字段列表</param>
        public async Task<long> HDelAsync(string cacheKey, IList<string> fields = null) => await RedisProvider.HDelAsync(cacheKey, fields);

        /// <summary>
        /// Hash 获取field的数量
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <returns></returns>
        public async Task<long> HLenAsync(string cacheKey) => await RedisProvider.HLenAsync(cacheKey);

        #endregion

    }
}
