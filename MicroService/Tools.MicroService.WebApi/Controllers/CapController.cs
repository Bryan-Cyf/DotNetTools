using DotNetCore.CAP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tools.MicroService.WebApi.Controllers
{
    public class CapController : BaseController
    {
        private readonly ICapPublisher _capPublisher; // 发布事件接口

        public CapController(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        /// <summary>
        /// 根据Id修改
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Publish(string id)
        {
            await _capPublisher.PublishAsync("OrderService.CreateOrder", id);
            return Ok();
        }
    }

    public class CapConsumer : ICapSubscribe
    {
        [CapSubscribe("OrderService.CreateOrder")]
        public void Event(string id)
        {
            Console.WriteLine($"cap测试数据接收：{id}");
        }
    }
}
