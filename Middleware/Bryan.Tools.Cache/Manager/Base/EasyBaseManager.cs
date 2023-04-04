using EasyCaching.Core;
using System;
using System.Threading.Tasks;

namespace Tools.Cache
{
    /// <summary>
    /// EasyCaching缓存服务
    /// </summary>
    public abstract class EasyBaseManager : IEasyCache
    {
        public abstract string Name { get; }

        /// <summary>
        /// EasyCaching缓存提供器
        /// </summary>
        public IEasyCachingProvider Provider { get; set; }

        public EasyBaseManager(IEasyCachingProviderFactory factory)
        {
            Provider = factory.GetCachingProvider(Name);
        }

        #region 同步

        /// <summary>
        /// 是否存在指定键的缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        public bool Exists(string key)
        {
            return Provider.Exists(key);
        }

        /// <summary>
        /// 从缓存中获取数据，如果不存在，则执行获取数据操作并添加到缓存中
        /// </summary>
        /// <typeparam name="T">缓存数据类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="func">获取数据操作</param>
        /// <param name="expiration">过期时间间隔</param>
        public T Get<T>(string key, Func<T> func, TimeSpan? expiration = null)
        {
            var result = Provider.Get(key, func, GetExpiration(expiration));
            return result.Value;
        }

        /// <summary>
        /// 获取过期时间间隔
        /// </summary>
        private TimeSpan GetExpiration(TimeSpan? expiration)
        {
            expiration = expiration ?? TimeSpan.FromHours(12);
            return expiration ?? default(TimeSpan);
        }
        /// <summary>
        /// 获取过期时间间隔
        /// </summary>
        /// <param name="cacheTime">分钟</param>
        /// <returns></returns>
        private TimeSpan GetExpiration(int? cacheTime)
        {
            return cacheTime.HasValue ? TimeSpan.FromMinutes(cacheTime.Value) : TimeSpan.FromHours(12);
        }

        /// <summary>
        /// 当缓存数据不存在则添加，已存在不会添加，添加成功返回true
        /// </summary>
        /// <typeparam name="T">缓存数据类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">值</param>
        /// <param name="expiration">过期时间间隔</param>
        public bool Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            return Provider.TrySet(key, value, GetExpiration(expiration));
        }

        /// <summary>
        /// 添加缓存。如果已存在缓存，将覆盖
        /// </summary>
        /// <typeparam name="T">缓存数据类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">值</param>
        /// <param name="expiration">过期时间间隔</param>
        public void SetOrUpdate<T>(string key, T value, TimeSpan? expiration = null) => Provider.Set(key, value, GetExpiration(expiration));

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        public void Remove(string key)
        {
            Provider.Remove(key);
        }

        public T Get<T>(string key, Func<T> func, int? cacheTime = null)
        {
            var result = Provider.Get(key, func, GetExpiration(cacheTime));
            return result.Value;
        }

        public T Get<T>(string key)
        {
            var result = Provider.Get<T>(key);
            return result.Value;
        }

        #endregion

        #region 异步 Async

        /// <summary>
        /// 是否存在指定键的缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        public async Task<bool> ExistsAsync(string key) => await Provider.ExistsAsync(key);

        /// <summary>
        /// 从缓存中获取数据，如果不存在，则执行获取数据操作并添加到缓存中
        /// </summary>
        /// <typeparam name="T">缓存数据类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="func">获取数据操作</param>
        /// <param name="expiration">过期时间间隔</param>
        public async Task<T> GetAsync<T>(string key, Func<Task<T>> func, TimeSpan? expiration = null)
        {
            var result = await Provider.GetAsync(key, func, GetExpiration(expiration));
            return result.Value;
        }

        /// <summary>
        /// 从缓存中获取数据
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="type">缓存数据类型</param>
        public async Task<object> GetAsync(string key, Type type)
        {
            return await Provider.GetAsync(key, type);
        }

        /// <summary>
        /// 从缓存中获取数据
        /// </summary>
        /// <typeparam name="T">缓存数据类型</typeparam>
        /// <param name="key">缓存键</param>
        public async Task<T> GetAsync<T>(string key)
        {
            var result = await Provider.GetAsync<T>(key);
            return result.Value;
        }

        /// <summary>
        /// 当缓存数据不存在则添加，已存在不会添加，添加成功返回true
        /// </summary>
        /// <typeparam name="T">缓存数据类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">值</param>
        /// <param name="expiration">过期时间间隔</param>
        public async Task<bool> TryAddAsync<T>(string key, T value, TimeSpan? expiration = null) => await Provider.TrySetAsync(key, value, GetExpiration(expiration));

        /// <summary>
        /// 添加缓存。如果已存在缓存，将覆盖
        /// </summary>
        /// <typeparam name="T">缓存数据类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="value">值</param>
        /// <param name="expiration">过期时间间隔</param>
        public async Task SetOrUpdateAsync<T>(string key, T value, TimeSpan? expiration = null) => await Provider.SetAsync(key, value, GetExpiration(expiration));

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        public async Task RemoveAsync(string key) => await Provider.RemoveAsync(key);

        /// <summary>
        /// 通过缓存键前缀移除缓存
        /// </summary>
        /// <param name="prefix">缓存键前缀</param>
        public async Task RemoveByPrefixAsync(string prefix) => await Provider.RemoveByPrefixAsync(prefix);

        #endregion
    }
}
