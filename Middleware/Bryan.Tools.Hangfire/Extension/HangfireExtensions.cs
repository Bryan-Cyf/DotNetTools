using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Tools.Hangfire
{
    internal static class HangfireExtensions
    {
        public static IApplicationBuilder UseHangfireService(this IApplicationBuilder app)
        {
            var config = app.ApplicationServices.GetService<IOptions<HangfireOptions>>().Value;
            if (config.IsOpenDashboard)
            {
                //自动清除作业历史
                if (config.AutoDeleteTaskLogDay > 0)
                {
                    GlobalJobFilters.Filters.Add(new LatencyTimeoutAttribute(config.AutoDeleteTaskLogDay * 60 * 60 * 24));
                }

                //自动重试功能
                GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = config.AutoRetryCount });

                DashboardOptions dashboardOptions = new DashboardOptions
                {
                    //只读设置
                    IsReadOnlyFunc = (DashboardContext context) => config.IsDashboardReadOnly,
                };

                if (config.IsOpenAuthentication)
                {
                    //声明鉴权过滤器
                    dashboardOptions.Authorization = new[]
                    {
                        new DashboardAuthorizationFilter()
                    };
                }
                //HangfireDashboard设置
                app.UseHangfireDashboard(config.RoutePrefix, dashboardOptions);

                HangfireConst.Password = config.Password;
            }

            return app;
        }

        public static IGlobalConfiguration UseStorage(this IGlobalConfiguration hangfireConfig, HangfireOptions options)
        {
            Enum.TryParse<StorageTypeEnum>(options.StorageType, out var type);
            switch (type)
            {
                case StorageTypeEnum.PostgreSql:
                    PostgreSqlStorageOptions hangfireDBOptions = new PostgreSqlStorageOptions
                    {
                        //数据库模式名称
                        SchemaName = "public",
                        //禁用自动创建数据库
                        PrepareSchemaIfNecessary = false,
                        //队列轮询间隔
                        QueuePollInterval = TimeSpan.FromSeconds(options.ScheduleInterval)
                    };
                    hangfireConfig.UsePostgreSqlStorage(options.Connection, hangfireDBOptions);
                    break;
                default:
                    hangfireConfig.UseMemoryStorage();
                    break;
            }
            return hangfireConfig;
        }
    }
}
