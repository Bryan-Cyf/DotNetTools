using EasyCaching.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tools.Cache
{
    public interface IEasyRedisCache : IEasyCache
    {

        /// <summary>
        /// 设置键过期时间
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="second">秒数</param>
        /// <returns></returns>
        Task<bool> KeyExpireAsync(string cacheKey, int second);

        /// <summary>
        /// Hash 获取指定字段的值
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        Task<string> HGetAsync(string cacheKey, string field);

        /// <summary>
        /// Hash 获取指定字段的值
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        Task<T> HGetAsync<T>(string cacheKey, string field) where T : class;

        /// <summary>
        /// Hash 覆盖设置指定键字段值
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="field"></param>
        /// <param name="cacheValue"></param>
        /// <returns></returns>
        Task<bool> HSetAsync(string cacheKey, string field, string cacheValue);

        /// <summary>
        /// Hash 增加指定字段的数值
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="field">值</param>
        /// <param name="val">增加数量</param>
        /// <returns>增值操作执行后的该字段的数值</returns>
        Task<long> HIncrByAsync(string cacheKey, string field, long val = 1);

        /// <summary>
        /// Hash 删除一个或多个指定域
        /// O(N)， N为要删除的域的数量
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="fields">删除字段列表</param>
        Task<long> HDelAsync(string cacheKey, IList<string> fields = null);

        /// <summary>
        /// Hash 获取field的数量
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <returns></returns>
        Task<long> HLenAsync(string cacheKey);


        /// <summary>
        ///根据前缀查询
        /// </summary>
        IDictionary<string, CacheValue<T>> GetByPrefix<T>(string key);

        Task<IDictionary<string, CacheValue<T>>> GetByPrefixAsync<T>(string key);

        /// <summary>
        ///根据前缀查询数量
        /// </summary>
        int GetCount<T>(string key = "");
        Task<int> GetCountAsync<T>(string key = "");
    }
}
