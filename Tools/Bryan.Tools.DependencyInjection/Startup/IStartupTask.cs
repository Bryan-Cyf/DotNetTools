using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tools.Dependency
{
    /// <summary>
    /// 在Startup.ConfigureServices后,IStartupFilter和Configure前,执行的异步任务
    /// 此任务未生成IApplicationBuilder,要使用请实现IStartupFilter
    /// </summary>
    public interface IStartupTask
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
