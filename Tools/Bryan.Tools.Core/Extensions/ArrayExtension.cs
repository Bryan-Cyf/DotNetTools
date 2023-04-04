using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 数组的扩展方法
/// </summary>
public static class ArrayExtension
{
    public static bool IsIn<T>(this T @this, params T[] list) => list.Contains(@this);
    public static async Task ForEachExt<T>(this IEnumerable<T> @this, Func<T, Task> func)
    {
        @this = @this ?? Array.Empty<T>();
        foreach (var item in @this)
        {
            await func(item);
        }
    }

    public static List<T2> ForEach<T1, T2>(this IEnumerable<T1> @this, Func<T1, T2> func)
    {
        @this = @this ?? Array.Empty<T1>();
        var result = new List<T2>();
        foreach (var item in @this)
        {
            var value = func(item);
            if (value != null)
                result.Add(value);
        }
        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="this"></param>
    /// <param name="func"></param>
    /// <returns>返回的值不包含null值</returns>
    public static async Task<List<T2>> ForEach<T1, T2>(this IEnumerable<T1> @this, Func<T1, Task<T2>> func)
    {
        @this = @this ?? Array.Empty<T1>();
        var result = new List<T2>();
        foreach (var item in @this)
        {
            var value = await func(item);
            if (value != null)
                result.Add(value);
        }
        return result;
    }

    public static async Task ForEach<T>(this IEnumerable<T> @this, Func<T, int, Task> func)
    {
        @this = @this ?? Array.Empty<T>();
        for (int i = 0; i < @this.Count(); i++)
        {
            await func(@this.ElementAt(i), i);
        }
    }

    public static async Task ForEachAsParallel<T>(this IEnumerable<T> @this, Func<T, Task> func, int? qps = null)
    {
        @this = @this ?? Array.Empty<T>();
        qps = qps ?? Environment.ProcessorCount * 2 + 2;
        List<Task> tasks = new List<Task>();
        foreach (var item in @this)
        {
            tasks.Add(func(item));
            if (tasks.Count == qps)
            {
                await Task.WhenAll(tasks);
                tasks.Clear();
            }
        }
        if (tasks.Count > 0)
            await Task.WhenAll(tasks);
    }

    public static async Task<IEnumerable<T2>> ForEachAsParallel<T1, T2>(this IEnumerable<T1> @this, Func<T1, T2> func, int? qps = null)
    {
        @this = @this ?? Array.Empty<T1>();
        qps = qps ?? Environment.ProcessorCount * 2 + 2;
        List<T2> result = new List<T2>();
        List<Task<T2>> tasks = new List<Task<T2>>();
        foreach (var item in @this)
        {
            tasks.Add(Task.Run(() => func(item)));
            if (tasks.Count == qps)
            {
                result.AddRange(await Task.WhenAll(tasks));
                tasks.Clear();
            }
        }
        if (tasks.Count > 0)
        {
            result.AddRange(await Task.WhenAll(tasks));
        }
        return result;
    }

    public static async Task<IEnumerable<T2>> ForEachAsParallel<T1, T2>(this IEnumerable<T1> @this, Func<T1, Task<T2>> func, int? qps = null)
    {
        @this = @this ?? Array.Empty<T1>();
        qps = qps ?? Environment.ProcessorCount * 2 + 2;
        List<T2> result = new List<T2>();
        List<Task<T2>> tasks = new List<Task<T2>>();
        foreach (var item in @this)
        {
            tasks.Add(func(item));
            if (tasks.Count == qps)
            {
                result.AddRange(await Task.WhenAll(tasks));
                tasks.Clear();
            }
        }
        if (tasks.Count > 0)
        {
            result.AddRange(await Task.WhenAll(tasks));
        }
        return result;
    }

}
