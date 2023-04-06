using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Tools.Mongo;

namespace Bryan.Demo.WebApi.Controllers
{
    public class MongoController : BaseController
    {
        //实现方式1
        private OrderMongoRepository _mongoRepository { get; set; }

        //实现方式2—等效的
        private IMongoRepository<OrderInfoEntity> _orderMongoRepository { get; set; }

        public MongoController(OrderMongoRepository mongoRepository, IMongoRepository<OrderInfoEntity> orderMongoRepository)
        {
            _mongoRepository = mongoRepository;
            _orderMongoRepository = orderMongoRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] OrderInfoEntity model)
        {
            //方式1
            await _orderMongoRepository.InsertOneAsync(model);

            //方式2—等效
            //await _mongoRepository.InsertOneAsync(model);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Query([FromQuery] string orderId)
        {
            var query1 = _mongoRepository.AsQueryable().FirstOrDefault(x => x.OrderId == orderId);

            var query2 = _mongoRepository.AsQueryable().Where(x => x.OrderId == orderId).Skip(0).Take(1).FirstOrDefault();

            var query3 = await _mongoRepository.FirstOrDefaultAsync(x => x.OrderId == orderId);

            var query4 = await _mongoRepository.FindAsync(x => x.OrderId == orderId);

            return Ok(query1);
        }
    }

    [MongoTable("order")]
    public class OrderInfoEntity : MongoEntity
    {
        public string OrderId { get; set; }
        public string Name { get; set; }
        public List<OrderItem> Items { get; set; }
    }

    public class OrderItem
    {
        public string ItemId { get; set; }
    }

    public class OrderMongoRepository : MongoBaseRepository<OrderInfoEntity>
    {
        public OrderMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {

        }
        public async override Task CreateIndexesAsync()
        {
            //唯一索引
            await IndexesCreateOneAsync("Name_1", x => x.Ascending(x => x.Name), Unique: true);

            //二级索引
            await IndexesCreateOneAsync("oldId_1_Items.ItemId_1", x => x.Ascending(x => x.OrderId).Ascending($"{nameof(OrderInfoEntity.Items)}.{nameof(OrderItem.ItemId)}"));
        }
    }
}
