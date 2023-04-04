using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tools.MicroService.Consul
{
    /// <summary>
    /// 服务发现
    /// </summary>
    public interface IServiceDiscovery
    {
        /// <summary>
        /// 服务发现
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns></returns>
        List<ServiceNode> Discovery(string serviceName);

        /// <summary>
        /// 服务刷新
        /// </summary>
        /// <returns></returns>
       void Refresh();
    }
}
