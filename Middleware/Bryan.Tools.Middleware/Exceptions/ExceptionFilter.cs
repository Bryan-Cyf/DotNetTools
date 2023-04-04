using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using Tools.Middleware.Buffering;
namespace Tools.Middleware.Exceptions
{
    /// <summary>
    /// 捕获并处理控制器方法中的异常
    /// </summary>
    internal class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var parameters = context.HttpContext.GetHttpParameters();
            var requestInfo = string.Format("[URL]:{0}-[Parameters]:{1}-[Begin]:{2}", context.HttpContext.Request.Path, parameters, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            _logger.LogError(context.Exception, requestInfo);

            context.HttpContext.Response.ContentType = "text/json;charset=utf-8;";
            context.Result = new BadRequestObjectResult("系统繁忙，请稍后再试");
            context.ExceptionHandled = true;
        }
    }
}
