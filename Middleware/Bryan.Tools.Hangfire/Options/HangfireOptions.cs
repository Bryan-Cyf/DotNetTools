using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Hangfire;

namespace Tools.Hangfire
{
    public class HangfireOptions
    {

        public const string SectionName = "Hangfire";

        /// <summary>
        /// 是否开启服务
        /// </summary>
        public bool IsOpenServer { get; set; }

        /// <summary>
        /// 是否开启面板
        /// </summary>
        public bool IsOpenDashboard { get; set; }

        /// <summary>
        /// 面板是否只读 -- 除了开发环境，其他环境都推荐只读
        /// </summary>
        public bool IsDashboardReadOnly { get; set; }

        /// <summary>
        /// URL后缀 -- /hangfire
        /// </summary>
        public string RoutePrefix { get; set; } = "/hangfire";

        /// <summary>
        /// 数据库链接
        /// </summary>
        public string Connection { get; set; }

        /// <summary>
        /// 服务名
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// 计划轮询时间 单位秒
        /// </summary>
        public int ScheduleInterval { get; set; } = 15;

        /// <summary>
        /// 指定队列 默认default
        /// </summary>
        public string[] Queues { get; set; }

        /// <summary>
        /// 工作者数量
        /// </summary>
        public int WorkCount { get; set; }

        /// <summary>
        /// 自动删除任务时间，单位秒
        /// </summary>
        public int AutoDeleteTaskLogDay { get; set; }

        /// <summary>
        /// 自动重试次数
        /// </summary>
        public int AutoRetryCount { get; set; }

        /// <summary>
        /// 是否开启登陆验证
        /// domain/SuffixUrl?pwd=Password
        /// </summary>
        public bool IsOpenAuthentication { get; set; }

        /// <summary>
        /// 登陆密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 存储形式
        /// </summary>
        public string StorageType { get; set; } = "Memory";

        /// <summary>
        /// 默认的userName
        /// </summary>
        public static string DefaultUserName => $"Host({Environment.MachineName})_PID({Process.GetCurrentProcess().Id})";

    }
}
