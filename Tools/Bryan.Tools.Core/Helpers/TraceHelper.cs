using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MagpieBridge.Common.Trace
{
    public static class TraceHelper
    {
        public static TraceContext Build(ILogger logger, [CallerMemberName] string callerMemberName = "")
        {
            var ctx = new TraceContext(logger, callerMemberName);
            ctx.Start();
            return ctx;
        }

        public static TraceContext Build(ILogger logger, int merchantId, [CallerMemberName] string callerMemberName = "")
        {
            var ctx = new TraceContext(logger, merchantId, callerMemberName);
            ctx.Start();
            return ctx;
        }
    }

    public class TraceContext : Stopwatch
    {
        private ILogger _logger;
        private DateTime _startTime = DateTime.Now;
        private string _callerMemberName;
        private StringBuilder _sb;
        private bool _close = false;

        public TraceContext(ILogger logger, string callerMemberName)
        {
            _logger = logger;
            _callerMemberName = callerMemberName;
            _sb = new StringBuilder(_callerMemberName);
        }
        public TraceContext(ILogger logger, int merchantId, string callerMemberName)
        {
            _logger = logger;
            _callerMemberName = merchantId + "-" + callerMemberName;
            _sb = new StringBuilder(_callerMemberName);
        }

        /// <summary>
        /// Trace总耗时 毫秒
        /// </summary>
        public double TotalMilliseconds => (DateTime.Now - _startTime).TotalMilliseconds;

        /// <summary>
        /// Trace总耗时 秒
        /// </summary>
        public double TotalSeconds => (DateTime.Now - _startTime).TotalSeconds;

        public string CallerMemberName => _callerMemberName;

        /// <summary>
        /// 关闭
        /// </summary>
        /// <returns></returns>
        public TraceContext Close()
        {
            _close = true;
            return this;
        }

        /// <summary>
        /// 记录当前阶段的耗时
        /// 记录后会Restart Trace
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public double Span(string msg)
        {
            var second = this.Elapsed.TotalSeconds;
            this.Restart();
            if (!_close)
            {
                _sb.Append("|");
                _sb.Append(msg);
                _sb.Append("-");
                _sb.Append("耗时:");
                _sb.Append(second.ToString("f2"));
                _sb.Append("秒");
            }
            return second;
        }

        /// <summary>
        /// 提交记录信息
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <param name="isLog">是否记录提交的日志</param>
        public void Submit(TimeSpan timeSpan = default, bool isLog = true)
        {
            this.Stop();
            if (_close || !isLog) return;

            _sb.Insert(0, $"总耗时:{(DateTime.Now - _startTime).TotalSeconds:f2}秒|");

            var msg = _sb.ToString();
            if (timeSpan != default && (DateTime.Now - _startTime) > timeSpan)
            {
                _logger.LogWarning(msg);
            }
            else if (timeSpan == default)
            {
                _logger.LogInformation(msg);
            }
            _close = true;
            _sb = null;
        }

        public override string ToString()
        {
            if (_close) return "closed";

            var msg = _sb.ToString().Insert(0, $"总耗时:{(DateTime.Now - _startTime).TotalSeconds:f2}秒|");

            return msg;
        }

    }
}
