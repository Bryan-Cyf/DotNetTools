using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tools.Cache
{
    public class MemoryCacheHelper
    {
        public static IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            return _memoryCache.Get<T>(key);
        }

        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Get(string key)
        {
            return _memoryCache.Get(key)?.ToString();
        }

        /// <summary>
        /// 设置缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="relative">绝对过期时间，例如relative是10分钟，那么缓存在10分钟后过期</param>
        /// <param name="offset">滑动过期时间，例如offset是10分钟，最后一次访问的10分钟后过期</param>
        public static void Set(string key, object value, TimeSpan? relative = null, TimeSpan? offset = null)
        {
            if (relative != null)
            {
                //缓存绝对过期时间，例如relative是10分钟，那么缓存在10分钟后过期
                cacheInsertAddMinutes(key, value, relative.Value);
            }
            else if (offset != null)
            {
                //缓存相对过期，例如offset是10分钟，最后一次访问的10分钟后过期
                cacheInsertFromMinutes(key, value, offset.Value);
            }
            else
            {
                //不设置过期时间
                cacheInsert(key, value);
            }
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        ///<param name="key"></param>
        public static void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        /// <summary>
        /// 缓存绝对过期时间
        /// </summary>
        ///<param name="key"></param>
        ///<param name="value"></param>
        ///<param name="expired">绝对过期时间</param>
        private static void cacheInsertAddMinutes(string key, object value, TimeSpan expired)
        {
            _memoryCache.Set(key, value, new MemoryCacheEntryOptions().SetAbsoluteExpiration(expired));
        }


        /// <summary>
        /// 缓存相对过期，最后一次访问的minute分钟后过期
        /// </summary>
        ///<param name="key"></param>
        ///<param name="value"></param>
        ///<param name="expired">滑动过期时间</param>
        private static void cacheInsertFromMinutes(string key, object value, TimeSpan expired)
        {
            _memoryCache.Set(key, value, new MemoryCacheEntryOptions().SetSlidingExpiration(expired));
        }


        /// <summary>
        ///以key键值，把value赋给Cache[key].如果不主动清空，会一直保存在内存中.
        /// </summary>
        ///<param name="key">Cache键值</param>
        ///<param name="value">给Cache[key]赋的值</param>
        private static void cacheInsert(string key, object value)
        {
            _memoryCache.Set(key, value);
        }
    }
}
