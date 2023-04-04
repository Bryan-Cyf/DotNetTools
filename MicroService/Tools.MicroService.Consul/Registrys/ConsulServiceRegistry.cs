using Consul;
using Microsoft.Extensions.Options;
using System;

namespace Tools.MicroService.Consul
{
    /// <summary>
    /// consul服务注册实现
    /// </summary>
    public class ConsulServiceRegistry
    {
        // 服务注册选项
        public readonly ConsulRegistryOptions serviceRegistryOptions;

        public ConsulServiceRegistry(IOptions<ConsulRegistryOptions> options)
        {
            this.serviceRegistryOptions = options.Value;
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        public void Register()
        {
            // 1、创建consul客户端连接
            var consulClient = new ConsulClient(configuration =>
            {
                //1.1 建立客户端和服务端连接
                configuration.Address = new Uri(serviceRegistryOptions.ConsulAddress);
            });

            // 2、获取
            var uri = new Uri(serviceRegistryOptions.ServiceAddress);

            // 3、创建consul服务注册对象
            var registration = new AgentServiceRegistration()
            {
                ID = string.IsNullOrEmpty(serviceRegistryOptions.ServiceId) ? Guid.NewGuid().ToString() : serviceRegistryOptions.ServiceId,
                Name = serviceRegistryOptions.ServiceName,
                Address = uri.Host,
                Port = uri.Port,
                Tags = serviceRegistryOptions.ServiceTags,
                Check = new AgentServiceCheck
                {
                    // 3.1、consul健康检查超时间
                    Timeout = TimeSpan.FromSeconds(10),
                    // 3.2、服务停止5秒后注销服务
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                    // 3.3、consul健康检查地址
                    HTTP = $"{uri.Scheme}://{uri.Host}:{uri.Port}{serviceRegistryOptions.HealthCheckAddress}",
                    // 3.4 consul健康检查间隔时间
                    Interval = TimeSpan.FromSeconds(10),
                }
            };

            // 4、注册服务
            consulClient.Agent.ServiceRegister(registration).Wait();

            Console.WriteLine($"服务注册成功:{serviceRegistryOptions.ServiceAddress}");
            // 5、关闭连接
            consulClient.Dispose();
        }

        /// <summary>
        /// 注销服务
        /// </summary>
        /// <param name="serviceNode"></param>
        public void Deregister()
        {
            // 1、创建consul客户端连接
            var consulClient = new ConsulClient(configuration =>
            {
                //1.1 建立客户端和服务端连接
                configuration.Address = new Uri(serviceRegistryOptions.ConsulAddress);
            });

            // 2、注销服务
            consulClient.Agent.ServiceDeregister(serviceRegistryOptions.ServiceId).Wait();


            Console.WriteLine($"服务注销成功:{serviceRegistryOptions.ServiceAddress}");

            // 3、关闭连接
            consulClient.Dispose();
        }

    }
}
