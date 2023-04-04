using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tools.MicroService.CAP
{
    internal static class DiHelper
    {
        public static IEnumerable<Type> GetTypes(params Type[] interfaces)
        {
            try
            {
                return from x in (from x in (from x in Directory.GetFiles(AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory, "*.dll", SearchOption.TopDirectoryOnly)
                                             select x.Split(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? '\\' : '/').Last().Replace(".dll", "") into x
                                             where !Regex.IsMatch(x, "^(?:Microsoft|System|runtime)") && !Regex.IsMatch(x, "resources$")
                                             select x).Select(delegate (string x)
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
                                  where x != null
                                  select x).SelectMany(delegate (Assembly x)
                                  {
                                      try
                                      {
                                          return x.GetTypes();
                                      }
                                      catch (Exception)
                                      {
                                          return new Type[0];
                                      }
                                  })
                       where x.IsClass && !x.IsAbstract && interfaces.Any((Type i) => i.IsAssignableFrom(x))
                       select x;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now} 加载类型异常\r\n" + ex.ToString());
                return null;
            }
        }
    }
}
