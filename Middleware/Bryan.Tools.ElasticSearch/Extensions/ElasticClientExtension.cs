using System;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Nest;

namespace Tools.Elastic
{
    public static class ElasticClientExtension
    {
        public static async Task CreateIndexAsync<T>(this ElasticClient elasticClient, string indexName = "", int numberOfShards = 5, int numberOfReplicas = 1) where T : class
        {
            if (string.IsNullOrWhiteSpace(indexName)) throw new ArgumentException("索引名称不可为空");

            if (!(await elasticClient.Indices.ExistsAsync(indexName)).Exists)
            {
                var indexState = new IndexState
                {
                    Settings = new IndexSettings
                    {
                        NumberOfReplicas = numberOfReplicas,
                        NumberOfShards = numberOfShards,

                        // index.blocks.read_only：设为true,则索引以及索引的元数据只可读
                        // index.blocks.read_only_allow_delete：设为true，只读时允许删除。
                        // index.blocks.read：设为true，则不可读。
                        // index.blocks.write：设为true，则不可写。
                        // index.blocks.metadata：设为true，则索引元数据不可读写
                    }
                };
                var response = await elasticClient.Indices.CreateAsync(indexName, p => p.InitializeUsing(indexState).Map<T>(x => x.AutoMap()));
                if (!response.IsValid)
                    throw new Exception($"创建索引失败:{response.OriginalException.Message}");
            }
        }

        public static ElasticClient CreateClient(this IConfiguration configuration)
        {
            var config = configuration.GetSection(ElasticOptions.SectionName).Get<ElasticOptions>();

            var uris = config.Uris;
            ConnectionSettings connectionSetting;
            if (uris.Count == 1)
            {
                var uri = uris.First();
                connectionSetting = new ConnectionSettings(uri);
            }
            else
            {
                var connectionPool = new SniffingConnectionPool(uris);
                connectionSetting = new ConnectionSettings(connectionPool);
            }

            if (!string.IsNullOrWhiteSpace(config.UserName) && !string.IsNullOrWhiteSpace(config.Password))
                connectionSetting.BasicAuthentication(config.UserName, config.Password);

            var client = new ElasticClient(connectionSetting);
            return client;
        }
    }
}