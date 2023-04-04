using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tools.Dependency
{
    /// <summary>
    /// 通过全局静态方法使用IServiceProvider
    /// </summary>
    public static class DiHelper
    {
        private static object _locker = new object();
        internal static IServiceCollection _services;
        public static IServiceCollection Services
        {
            get
            {
                if (_services == null)
                {
                    lock (_locker)
                    {
                        if (_services == null)
                        {
                            _services = new ServiceCollection();
                        }
                    }
                }
                return _services;
            }
        }

        internal static IServiceProvider _serviceProvider;
        public static IServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider == null)
                {
                    lock (_locker)
                    {
                        if (_serviceProvider == null && Services != null)
                        {
                            _serviceProvider = Services.BuildServiceProvider();
                        }
                    }
                }
                if (_serviceProvider == null) throw new ArgumentNullException("请使用UseDiHelper或者AddDiHelper来初始化DiHelper");
                return _serviceProvider;
            }
        }

        public static T GetRequiredService<T>() => ServiceProvider.GetRequiredService<T>();
        public static object GetRequiredService(Type type) => ServiceProvider.GetRequiredService(type);
        public static T GetService<T>() => ServiceProvider.GetService<T>();
        public static object GetService(Type type) => ServiceProvider.GetService(type);
        public static IEnumerable<T> GetServices<T>() => ServiceProvider.GetServices<T>();
        public static IEnumerable<object> GetServices(Type type) => ServiceProvider.GetServices(type);

        /// <summary>
        /// 配置
        /// </summary>
        public static IConfiguration Configuration => ServiceProvider.GetRequiredService<IConfiguration>();
        /// <summary>
        /// 获取配置类实体
        /// </summary>
        public static T GetConfiguration<T>(string key = default) where T : class => string.IsNullOrEmpty(key) ? Configuration.Get<T>() : Configuration.GetSection(key).Get<T>();

        /// <summary>
        /// httpclient工厂
        /// </summary>
        public static IHttpClientFactory HttpClientFactory => GetService<IHttpClientFactory>();
        /// <summary>
        /// 获取httpclient
        /// </summary>
        public static HttpClient GetHttpClient(string name = default) => string.IsNullOrEmpty(name) ? HttpClientFactory.CreateClient() : HttpClientFactory.CreateClient(name);

        /// <summary>
        /// 先注册ILoggerFactory
        /// </summary>
        public static ILoggerFactory LoggerFactory => ServiceProvider.GetRequiredService<ILoggerFactory>();
        public static ILogger<T> GetLogger<T>() => LoggerFactory.CreateLogger<T>();
        public static ILogger GetLogger(string name) => LoggerFactory.CreateLogger(name);
        public static ILogger GetLogger(Type type) => LoggerFactory.CreateLogger(type);

        /// <summary>
        /// 内存缓存,先注册IMemoryCache
        /// </summary>
        public static IMemoryCache MemoryCache => ServiceProvider.GetRequiredService<IMemoryCache>();

        /// <summary>
        /// 数据库连接,先注册IDbConnection
        /// </summary>
        public static IDbConnection DbConnection => ServiceProvider.GetRequiredService<IDbConnection>();

        /// <summary>
        /// 分布式缓存,先注册IDistributedCache
        /// </summary>
        public static IDistributedCache DistributedCache => ServiceProvider.GetRequiredService<IDistributedCache>();

        /// <summary>
        /// 增加gb2312等编码的支持
        /// </summary>
        public static void EncodingRegisterProvider()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        /// 查找所有实现了指定接口的类型
        /// </summary>
        public static IEnumerable<Type> GetTypes(params Type[] interfaces)
        {
            try
            {
                var path = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
                var allFiles = Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly)
                    .Select(x => x.Split(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? '\\' : '/').Last().Replace(".dll", ""))
                    .Where(x => !Regex.IsMatch(x, @"^(?:Microsoft|System|Tools|runtime)") && !Regex.IsMatch(x, "resources$"));
                var types = allFiles
                    .Select(x =>
                    {
                        try
                        {
                            return Assembly.Load(new AssemblyName(x));
                        }
                        catch (Exception)
                        {
                            return null;
                        }
                    })
                    .Where(x => x != null)
                    .SelectMany(x =>
                    {
                        try
                        {
                            return x.GetTypes();
                        }
                        catch (Exception)
                        {
                            return new Type[0] { };
                        }
                    })
                    .Where(x => x.IsClass && !x.IsAbstract && interfaces.Any(i => i.IsAssignableFrom(x)));
                return types;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} 加载类型异常\r\n" + ex.ToString());
                return null;
            }
        }

        public static IEnumerable<Type> GetTypes2(params Type[] interfaces)
        {
            try
            {
                var types = GetAllAssembly()
                    .SelectMany(x =>
                    {
                        try
                        {
                            return x.GetTypes();
                        }
                        catch (Exception)
                        {
                            return new Type[0] { };
                        }
                    })
                    .Where(x => x.IsClass && !x.IsAbstract && interfaces.Any(i => i.IsAssignableFrom(x)));
                return types;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} 加载类型异常\r\n" + ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 在指定的程序集查找所有实现了指定接口的类型
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
        /// <param name="type">接口名称</param>
        public static IEnumerable<Type> GetTypes(string assemblyName, Type type)
        {
            var allTypes = Assembly.Load(new AssemblyName(assemblyName)).GetTypes()
                                .Where(x => x.IsClass && !x.IsAbstract && type.IsAssignableFrom(x));
            return allTypes;
        }

        /// <summary>
        /// 在指定的程序集查找所有实现了指定接口的类型
        /// </summary>
        /// <param name="assemblyNames"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetTypes(string[] assemblyNames, Type type)
        {
            var allTypes = assemblyNames.Select(x => Assembly.Load(new AssemblyName(x)).GetTypes())
                .SelectMany(x => x)
                .Where(x => x.IsClass && !x.IsAbstract && type.IsAssignableFrom(x));
            return allTypes;
        }

        public static IEnumerable<Type> GetTypes(string[] assemblyNames, Type[] types)
        {
            var allTypes = assemblyNames.Select(x => Assembly.Load(new AssemblyName(x)).GetTypes())
                .SelectMany(x => x)
                .Where(x => x.IsClass && !x.IsAbstract && types.Any(type => type.IsAssignableFrom(x)));
            return allTypes;
        }

        /// <summary>
        /// 获取所有程序集
        /// </summary>
        /// <returns></returns>
        public static List<Assembly> GetAllAssembly()
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            HashSet<string> loadedAssemblies = new HashSet<string>();
            foreach (var item in allAssemblies)
            {
                loadedAssemblies.Add(item.FullName!);
            }

            Queue<Assembly> assembliesToCheck = new Queue<Assembly>();
            assembliesToCheck.Enqueue(Assembly.GetEntryAssembly()!);
            while (assembliesToCheck.Any())
            {
                var assemblyToCheck = assembliesToCheck.Dequeue();
                foreach (var reference in assemblyToCheck!.GetReferencedAssemblies())
                {
                    if (!loadedAssemblies.Contains(reference.FullName))
                    {
                        var assembly = Assembly.Load(reference);
                        assembliesToCheck.Enqueue(assembly);
                        loadedAssemblies.Add(reference.FullName);
                        allAssemblies.Add(assembly);
                    }
                }
            }

            return allAssemblies;
        }
    }
}
