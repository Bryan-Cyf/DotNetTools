using Hangfire;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Hangfire
{
    /// <summary>
    /// 自动周期性任务基类
    /// </summary>
    public abstract class AutoReccurringBase : IHangfireReccuring
    {
        public abstract string Name { get; }

        public abstract string Cron { get; }

        public virtual string Queue { get; set; } = "default";

        public virtual bool IsValid { get; set; } = true;

        public abstract Task ExcuteAsync();

        public void Handler()
        {
            if (IsValid && !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Cron))
            {
                RecurringJob.AddOrUpdate(Name, () => ExcuteAsync(), Cron, queue: Queue);
            }
        }
    }
}
