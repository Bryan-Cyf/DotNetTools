using System;
using System.Collections.Generic;
using System.Text;

namespace Tools.Cache
{
    public class CacheOptions
    {
        public const string SectionName = "Cache";

        /// <summary>
        /// 是否启用Redis
        /// </summary>
        public bool IsUseRedis { get; set; } = true;

        /// <summary>
        /// Redis链接字符串
        /// </summary>
        public List<string> Connections { get; set; }

        /// <summary>
        /// 内存限制大小-KeyValue数据量
        /// </summary>
        public int? MemorySizeLimit { get; set; } = 2000;
    }
}
