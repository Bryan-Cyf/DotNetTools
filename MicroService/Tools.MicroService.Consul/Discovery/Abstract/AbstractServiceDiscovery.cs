using Consul;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;

namespace Tools.MicroService.Consul
{
    /// <summary>
    /// 抽象服务发现，主要是缓存功能
    /// </summary>
    public abstract class AbstractServiceDiscovery : IServiceDiscovery
    {
        // 字典缓存
        private readonly Dictionary<string, List<ServiceNode>> CacheConsulResult = new Dictionary<string, List<ServiceNode>>();
        protected readonly ConsulDiscoveryOptions serviceDiscoveryOptions;
        public AbstractServiceDiscovery(IOptions<ConsulDiscoveryOptions> options)
        {
            this.serviceDiscoveryOptions = options.Value;

            // 1、创建consul客户端连接
            var consulClient = new ConsulClient(configuration =>
            {
                //1.1 建立客户端和服务端连接
                configuration.Address = new Uri(serviceDiscoveryOptions.DiscoveryAddress);
            });

            // 2、consul 先查询服务
            var queryResult = consulClient.Catalog.Services().Result;
            if (!queryResult.StatusCode.Equals(HttpStatusCode.OK))
            {
                throw new Exception($"consul连接失败:{queryResult.StatusCode}");
            }

            // 3、获取服务下的所有实例
            foreach (var item in queryResult.Response)
            {
                QueryResult<CatalogService[]> result = consulClient.Catalog.Service(item.Key).Result;
                if (!queryResult.StatusCode.Equals(HttpStatusCode.OK))
                {
                    throw new Exception($"consul连接失败:{queryResult.StatusCode}");
                }
                var list = new List<ServiceNode>();
                foreach (var service in result.Response)
                {
                    list.Add(new ServiceNode { Url = service.ServiceAddress + ":" + service.ServicePort });
                }
                CacheConsulResult.Add(item.Key, list);
            }
        }


        public List<ServiceNode> Discovery(string serviceName)
        {
            // 1、从缓存中查询consulj结果
            if (CacheConsulResult.ContainsKey(serviceName))
            {
                return CacheConsulResult[serviceName];
            }
            else
            {
                // 1.2、从远程服务器取
                CatalogService[] queryResult = RemoteDiscovery(serviceName);

                var list = new List<ServiceNode>();
                foreach (var service in queryResult)
                {
                    list.Add(new ServiceNode { Url = service.ServiceAddress + ":" + service.ServicePort });
                }

                // 1.3 将结果添加到缓存
                CacheConsulResult.Add(serviceName, list);

                return list;
            }
        }

        /// <summary>
        /// 远程服务发现
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        protected abstract CatalogService[] RemoteDiscovery(string serviceName);

        public void Refresh()
        {
            throw new NotImplementedException();
        }
    }
}
