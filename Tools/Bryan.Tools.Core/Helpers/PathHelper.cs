using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Tools
{
    public static class PathHelper
    {
        public static string AppGuid
        {
            get
            {
                try
                {
                    Assembly asm = Assembly.GetEntryAssembly();
                    object[] attr = (asm.GetCustomAttributes(typeof(GuidAttribute), true));
                    return new Guid((attr[0] as GuidAttribute).Value).ToString("B").ToUpper();
                }
                catch (Exception)
                {
                    return AppDomain.CurrentDomain.FriendlyName;
                }
            }
        }

        /// <summary>
        /// 获取临时目录的路径
        /// </summary>
        /// <param name="appendName"></param>
        /// <returns></returns>
        public static string GetTempPath(string appendName = default)
        {
            if (string.IsNullOrWhiteSpace(appendName)) appendName = AppGuid;
            return Path.Combine(System.IO.Path.GetTempPath(), appendName);
        }
    }
}
