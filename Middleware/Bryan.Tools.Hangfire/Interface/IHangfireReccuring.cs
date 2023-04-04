using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Hangfire
{
    public interface IHangfireReccuring
    {
        string Name { get; }

        string Cron { get; }

        string Queue { get; set; }

        bool IsValid { get; set; }

        Task ExcuteAsync();

        void Handler();
    }
}
