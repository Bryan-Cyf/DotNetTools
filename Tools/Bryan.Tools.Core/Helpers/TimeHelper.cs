using System;

namespace Tools
{
    /// <summary>
    /// 时间帮助类
    /// </summary>
    public class TimeHelper
    {
        /// <summary>
        /// 获取时间戳,单位毫秒
        /// </summary>
        /// <returns>毫秒</returns>
        public static long GetMilliTimestanp()
        {
            return (DateTimeOffset.Now.UtcTicks - 621355968000000000) / 10000;
        }

        /// <summary>
        /// 获取时间戳,单位秒
        /// </summary>
        /// <returns>秒</returns>
        public static long GetTimestanp()
        {
            return GetMilliTimestanp() / 1000;
        }
    }
}
