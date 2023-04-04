using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Tools.Middleware.Buffering;

namespace Tools.Middleware.Exceptions
{
    /// <summary>
    /// 拦截请求并处理异常
    /// </summary>
    internal class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            this.next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (Exception ex)
            {
                var parameters = context.GetHttpParameters();
                var requestInfo = string.Format("[URL]:{0}-[Parameters]:{1}-[Begin]:{2}", context.Request.Path, parameters, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                _logger.LogError(ex, requestInfo);

                context.Response.ContentType = "text/json;charset=utf-8;";
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("系统繁忙，请稍后再试");
            }
        }
    }
}
