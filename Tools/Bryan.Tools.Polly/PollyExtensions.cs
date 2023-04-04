using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Polly.Bulkhead;

namespace Helpers
{
    public static class PollyExtensions
    {
        public static async Task BulkheadAsync<T>(this IEnumerable<T> items, Func<T, Task> callback, int? maxParallelization = default)
        {
            if (items == null || items.Count() == 0) return;
            var polly = BulkheadPolicy.BulkheadAsync(maxParallelization ?? Environment.ProcessorCount);
            await Task.WhenAll(items.Select(x => polly.ExecuteAsync(() => callback(x))));
        }

        public static async Task<IEnumerable<T2>> BulkheadAsync<T1, T2>(this IEnumerable<T1> items, Func<T1, Task<T2>> callback, int? maxParallelization = default)
        {
            if (items == null || items.Count() == 0) return null;
            var polly = BulkheadPolicy.BulkheadAsync(maxParallelization ?? Environment.ProcessorCount, int.MaxValue);
            return await Task.WhenAll(items.Select(x => polly.ExecuteAsync(() => callback(x))));
        }
    }
}
