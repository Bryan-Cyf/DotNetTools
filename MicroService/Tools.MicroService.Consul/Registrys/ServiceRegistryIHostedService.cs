using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Tools.MicroService.Consul
{
    /// <summary>
    /// 服务启动时自动注册
    /// </summary>
    public class ServiceRegistryIHostedService : IHostedService
    {
        private readonly ConsulServiceRegistry serviceRegistry;

        public ServiceRegistryIHostedService(ConsulServiceRegistry serviceRegistry)
        {
            this.serviceRegistry = serviceRegistry;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => serviceRegistry.Register());
        }

        /// <summary>
        /// 服务停止时注销
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            serviceRegistry.Deregister();
            return Task.CompletedTask;
        }
    }
}
