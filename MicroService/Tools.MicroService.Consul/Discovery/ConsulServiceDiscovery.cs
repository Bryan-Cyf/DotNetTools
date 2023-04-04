using Consul;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;

namespace Tools.MicroService.Consul
{
    /// <summary>
    /// consul服务发现实现
    /// </summary>
    public class ConsulServiceDiscovery : IServiceDiscovery
    {
        // 1、服务地址缓存
        private readonly Dictionary<string, List<ServiceNode>> CacheConsulResult = new Dictionary<string, List<ServiceNode>>();
        //  选择ConcurrentDictionary

        private readonly ConsulDiscoveryOptions serviceDiscoveryOptions;

        // 初始化(目的：consul中的服务地址加载到缓存。consul客户端连接屏蔽，达到提升性能目的)
        public ConsulServiceDiscovery(IOptions<ConsulDiscoveryOptions> options)
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

                // 4、加载到字典缓存
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

            // 1.2、从远程服务器取
            CatalogService[] queryResult = RemoteDiscovery(serviceName);

            var list = new List<ServiceNode>();
            foreach (var service in queryResult)
            {
                list.Add(new ServiceNode { Url = service.ServiceAddress + ":" + service.ServicePort });
            }

            return list;
        }

        private CatalogService[] RemoteDiscovery(string serviceName)
        {
            // 1、创建consul客户端连接
            var consulClient = new ConsulClient(configuration =>
            {
                //1.1 建立客户端和服务端连接
                configuration.Address = new Uri(serviceDiscoveryOptions.DiscoveryAddress);
            });

             // 方案1：全局通用：可以提升性能
             // 缺陷：连接通用：网络请求不通用。消耗时间
             // 方案2:
             // 内存缓存：基于内存操作。
             // 字典，MemeroyCache

            // 2、consul查询服务,根据具体的服务名称查询
            var queryResult = consulClient.Catalog.Service(serviceName).Result;
            // 3、判断请求是否失败
            if (!queryResult.StatusCode.Equals(HttpStatusCode.OK))
            {
                throw new Exception($"consul连接失败:{queryResult.StatusCode}");
            }
            return queryResult.Response;
        }

        /// <summary>
        /// 服务本地缓存刷新
        /// </summary>

        public void Refresh()
        {
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
            CacheConsulResult.Clear();
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
    }
}
