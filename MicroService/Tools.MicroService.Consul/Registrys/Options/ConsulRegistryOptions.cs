using System;

namespace Tools.MicroService.Consul
{
    /// <summary>
    /// 节点注册选项
    /// </summary>
    public class ConsulRegistryOptions
    {
        public ConsulRegistryOptions()
        {
            this.ServiceId = Guid.NewGuid().ToString();
            this.ConsulAddress = "http://localhost:8500";
            this.HealthCheckAddress = "/HealthCheck";
        }

        public const string SectionName = "ConsulRegistry";

        // 服务ID
        public string ServiceId { get; set; }

        // 服务名称
        public string ServiceName { get; set; }

        // 服务地址http://localhost:5001
        public string ServiceAddress { get; set; }

        // 服务标签(版本)
        public string[] ServiceTags { set; get; }

        // Consul注册地址
        public string ConsulAddress { get; set; }

        // 服务健康检查地址
        public string HealthCheckAddress { get; set; }
    }
}
