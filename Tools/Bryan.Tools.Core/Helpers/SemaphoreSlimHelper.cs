using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tools
{
    /// <summary>
    /// 根据不同的key进行同步
    /// </summary>
    public class SemaphoreSlimHelper
    {
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _lockers = new ConcurrentDictionary<string, SemaphoreSlim>();
        public static async Task WaitAsync(string key, int count = 1)
        {
            if (!_lockers.TryGetValue(key, out var slim))
            {
                lock (key)
                {
                    slim = _lockers.GetOrAdd(key, new SemaphoreSlim(count));
                }
            }
            await slim.WaitAsync();
        }

        public static void Release(string key)
        {
            if (_lockers.TryGetValue(key, out var slim))
            {
                slim.Release();
            }
        }

        public static int Count(string key, int count = 1)
        {
            if (!_lockers.TryGetValue(key, out var slim))
            {
                lock (key)
                {
                    slim = _lockers.GetOrAdd(key, new SemaphoreSlim(count));
                }
            }
            return slim.CurrentCount;
        }

        public static async Task WaitAndReleaseAsync(string key, Func<Task> func, int count = 1)
        {
            if (func is null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            await WaitAsync(key, count);
            try
            {
                await func();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Release(key);
            }
        }
        public static async Task<T> WaitAndReleaseAsync<T>(string key, Func<Task<T>> func, int count = 1)
        {
            if (func is null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            await WaitAsync(key, count);
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Release(key);
            }
        }

        public static SemaphoreSlim Get(string key, int count)
        {
            return _lockers.GetOrAdd(key, x =>
            {
                return new SemaphoreSlim(count);
            });
        }
    }
}
