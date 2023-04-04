using System;

namespace Tools.MicroService.Consul
{
    public class HealthCheckOptions
    {
        public HealthCheckOptions()
        {
            this.HealthCheckAddress = "/HealthCheck";
        }

        // 服务健康检查地址
        public string HealthCheckAddress { get; set; }
    }
}
