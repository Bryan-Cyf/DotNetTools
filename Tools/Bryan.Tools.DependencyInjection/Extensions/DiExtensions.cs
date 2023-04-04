using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tools.Dependency;

public static class DiExtensions
{
    /// <summary>
    /// 初始化全局DiHelper单例,会创建一个新的IServiceProvider
    /// </summary>
    /// <param name="services">服务</param>
    /// <param name="SingletonAssemblies">实现了ISingleton,IDiScoped,IDiSingleton,IDiTransient的程序集名称集合</param>
    public static IServiceCollection AddDiHelper(this IServiceCollection services, string[] SingletonAssemblies = null)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        //自动注入ISingleton
        IEnumerable<Type> types = null;
        if (SingletonAssemblies?.Count() > 0)
        {
            types = DiHelper.GetTypes(SingletonAssemblies, new Type[] { typeof(IDiScoped), typeof(IDiSingleton), typeof(IDiTransient) });
        }
        else
        {
            types = DiHelper.GetTypes(typeof(IDiScoped), typeof(IDiSingleton), typeof(IDiTransient));
        }
        if (types?.Count() > 0)
        {
            foreach (var item in types)
            {
                if (typeof(IDiSingleton).IsAssignableFrom(item))
                {
                    services.TryAddSingleton(item);
                }
                else if (typeof(IDiScoped).IsAssignableFrom(item))
                {
                    services.TryAddScoped(item);
                }
                if (typeof(IDiTransient).IsAssignableFrom(item))
                {
                    services.TryAddTransient(item);
                }
            }
            Console.WriteLine($"注册单例--{string.Join(",", types.Select(x => x.Name))}");
        }

        //配置单例IServiceProvider
        services.AddSingleton<IStartupFilter, SetupSpStartupFilter>();

        return DiHelper._services = services;
    }

    /// <summary>
    /// 初始化全局DiHelper单例,并在启动时自动执行IStartupTask注册的服务,用在program.cs
    /// </summary>
    public static async Task RunWithDiHelperAsync(this IHost host, CancellationToken cancellationToken = default)
    {
        DiHelper._serviceProvider = host.Services;
        //注册中文编码
        DiHelper.EncodingRegisterProvider();

        //执行IStartupTask
        var startupTasks = host.Services.GetServices<IStartupTask>();
        foreach (var startupTask in startupTasks)
        {
            await startupTask.ExecuteAsync(cancellationToken);
        }

        await host.RunAsync(cancellationToken);
    }
}
