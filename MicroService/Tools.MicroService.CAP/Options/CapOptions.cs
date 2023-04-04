using DotNetCore.CAP;

namespace Tools.MicroService.CAP
{
    public class CapOptions
    {
        public CapOptions()
        {
            FailedRetryCount = 3;
            FailedRetryInterval = 60;
        }
        public const string SectionName = "Cap";

        public string HostName { get; set; }

        public string Password { get; set; }

        public string UserName { get; set; }

        public string VirtualHost { get; set; }

        public string ExchangeName { get; set; }

        public int Port { get; set; }

        /// <summary>
        /// 数据库持久化链接
        /// </summary>
        public string Connection { get; set; }

        /// <summary>
        /// 失败重试次数
        /// </summary>
        public int FailedRetryCount { get; set; }

        /// <summary>
        /// 失败重试间隔 单位秒
        /// </summary>
        public int FailedRetryInterval { get; set; }
    }
}