using MagpieBridge.Common.Trace;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Tools.Middleware.Buffering;

namespace Tools.Middleware.Monitor
{
    /// <summary>
    /// 全局Http请求耗时监控
    /// </summary>
    internal class TimeMonitorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly TimeMonitorOption _option;
        public TimeMonitorMiddleware(RequestDelegate next, ILogger<TimeMonitorMiddleware> logger, IOptions<TimeMonitorOption> option)
        {
            _next = next;
            _logger = logger;
            _option = option.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            var parameters = context.GetHttpParameters();
            var requestInfo = string.Format("[URL]:{0}-[Parameters]:{1}-[Begin]:{2}", context.Request.Path, parameters, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            var trace = TraceHelper.Build(_logger, requestInfo);
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                trace.Span($"Exception:{ex.Message}");
                throw ex;
            }
            finally
            {
                trace.Submit(TimeSpan.FromMilliseconds(_option.MilliSeconds));
            }
        }
    }
}
