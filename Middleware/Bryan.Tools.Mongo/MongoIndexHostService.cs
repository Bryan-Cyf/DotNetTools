using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tools.Mongo
{
    /// <summary>
    /// 自动创建和删除索引
    /// </summary>
    public class MongoIndexHostService : IHostedService
    {
        private readonly MongoOptions _mongoConfig;
        private readonly IServiceProvider _serviceProvider;

        public MongoIndexHostService(IOptionsMonitor<MongoOptions> mongoConfig, IServiceProvider serviceProvider)
        {
            _mongoConfig = mongoConfig.CurrentValue;
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_mongoConfig.IsAutoCreateIndex)
            {
                foreach (var type in MongoExtensions._mongoRepositoryTypes)
                {
                    var repository = _serviceProvider.GetRequiredService(type);
                    try
                    {
                        var createMethod = type.GetMethod(nameof(MongoBaseRepository<MongoEntity>.CreateIndexesAsync), BindingFlags.Instance | BindingFlags.Public);
                        var createTask = createMethod.Invoke(repository, null) as Task;
                        await createTask.ConfigureAwait(false);

                        var DropMethod = type.GetMethod(nameof(MongoBaseRepository<MongoEntity>.DropIndexesAsync), BindingFlags.Instance | BindingFlags.NonPublic);
                        var dropTask = DropMethod.Invoke(repository, null) as Task;
                        await dropTask.ConfigureAwait(false);
                    }
                    catch { }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("MongDB Stop");
            return Task.CompletedTask;
        }
    }
}
