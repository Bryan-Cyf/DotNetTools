using MongoDB.Driver;
using System.Threading.Tasks;

namespace Tools.Mongo
{
    /// <summary>
    /// 通用的Mongo仓储
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MongoRepository<T> : MongoBaseRepository<T> where T : MongoEntity
    {
        public MongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {

        }

        public override Task CreateIndexesAsync()
        {
            return Task.CompletedTask;
        }
    }
}
