using Nest;

namespace Tools.Elastic
{
    public class ElasticRepository<T> : ElasticBaseRepository<T> where T : ElasticEntity
    {
        public ElasticRepository(IElasticClient elasticClient) : base(elasticClient)
        {
        }
    }
}