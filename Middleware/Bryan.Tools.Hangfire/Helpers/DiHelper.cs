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

namespace Tools.Hangfire
{
    /// <summary>
    /// 通过全局静态方法使用IServiceProvider
    /// </summary>
    internal static class DiHelper
    {
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
                    .Where(x => !Regex.IsMatch(x, @"^(?:Microsoft|System|runtime)") && !Regex.IsMatch(x, "resources$"));
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
    }
}
