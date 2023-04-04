using Polly;
using Polly.Retry;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Helpers
{
    public class PollyHelper
    {
        private static ConcurrentDictionary<int, AsyncRetryPolicy> WaitAndRetryAsyncCache = new ConcurrentDictionary<int, AsyncRetryPolicy>();
        private static ConcurrentDictionary<int, RetryPolicy> WaitAndRetryCache = new ConcurrentDictionary<int, RetryPolicy>();
        public static ILogger _logger = Log.ForContext<PollyHelper>();

        public static T WaitAndRetry<T>(Func<T> callback, int retryCount = 10)
        {
            var policy = WaitAndRetryCache.GetOrAdd(retryCount, x =>
            Policy.Handle<Exception>()
            .WaitAndRetry(retryCount, xx => TimeSpan.FromSeconds(xx), (ex, time, index, context) =>
            {
                _logger.Warning(ex, $"第{index}次重试,睡眠{time.TotalSeconds}");
            }));
            return policy.Execute(() => callback());
        }

        public static Task<T> WaitAndRetryAsync<T>(Func<Task<T>> callback, int retryCount = 10)
        {
            var policy = GetAsyncRetryPolicy(retryCount);
            return policy.ExecuteAsync(() => callback());
        }

        public static async Task WaitAndRetryAsync(Func<Task> callback, int retryCount = 10)
        {
            var policy = GetAsyncRetryPolicy(retryCount);
            await policy.ExecuteAsync(() => callback());
        }

        private static AsyncRetryPolicy GetAsyncRetryPolicy(int key)
        {
            var policy = WaitAndRetryAsyncCache.GetOrAdd(key, _ =>
            Policy.Handle<Exception>()
            .WaitAndRetryAsync(key, x => TimeSpan.FromSeconds(1), (ex, time, index, context) =>
            {
                _logger.Warning(ex, $"第{index}次重试");
            }));
            return policy;
        }
    }
}
