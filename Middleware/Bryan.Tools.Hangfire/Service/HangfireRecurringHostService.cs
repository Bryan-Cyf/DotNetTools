using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tools.Hangfire
{
    /// <summary>
    /// 自动执行定时任务
    /// </summary>
    public class HangfireRecurringHostService : IHostedService
    {
        private readonly IEnumerable<IHangfireReccuring> _services;

        public HangfireRecurringHostService(IEnumerable<IHangfireReccuring> services)
        {
            _services = services;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var service in _services)
            {
                service.Handler();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Hangfire Stop");
            return Task.CompletedTask;
        }
    }
}
