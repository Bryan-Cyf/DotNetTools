using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Tools.Hangfire;

namespace Bryan.Demo.WebApi.Controllers
{
    public class TimerController : BaseController
    {
        public TimerController()
        {

        }

        /// <summary>
        /// 一次性任务
        /// </summary>
        [HttpGet]
        public void OnceTask()
        {
            BackgroundJob.Enqueue(() => Console.WriteLine("Simple!"));
        }

        /// <summary>
        /// 延迟任务
        /// </summary>
        [HttpGet]
        public void DelayedTask()
        {
            BackgroundJob.Schedule(() => Console.WriteLine("Reliable!"), TimeSpan.FromDays(7));
        }

        /// <summary>
        /// 周期性任务
        /// </summary>
        [HttpGet]
        public void RecurringTask()
        {
            RecurringJob.AddOrUpdate("test1", () => Console.WriteLine("Transparent!"), Cron.Minutely, queue: "test1");
            //立即执行周期任务
            RecurringJob.Trigger("test1");
        }
    }

    /// <summary>
    /// 自动注册周期性任务
    /// </summary>
    public class OrderReccurring : AutoReccurringBase
    {
        public override string Name => "Order";

        public override string Cron => Hangfire.Cron.Minutely();

        /// <summary>
        /// 指定执行队列 默认defualt
        /// </summary>
        /// <returns></returns>
        [Queue("test1")]
        public override Task ExcuteAsync()
        {
            Console.WriteLine("Transparent!");
            return Task.CompletedTask;
        }
    }
}
