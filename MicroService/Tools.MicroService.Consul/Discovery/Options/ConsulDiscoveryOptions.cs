namespace Tools.MicroService.Consul
{
    /// <summary>
    /// 服务发现选项
    /// </summary>
    public class ConsulDiscoveryOptions
    {
        public ConsulDiscoveryOptions()
        {

        }

        /// <summary>
        /// 服务发现地址
        /// </summary>
        public string DiscoveryAddress { set; get; }
    }
}
