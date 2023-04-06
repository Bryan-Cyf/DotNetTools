using Tools.Cache;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Bryan.Demo.WebApi.Controllers
{
    public class CacheController : BaseController
    {
        private readonly IEasyMemoryCache _memory;
        private readonly IEasyRedisCache _redis;
        private readonly IEasyHybirdCache _hybird;


        public CacheController(IEasyMemoryCache memory
            , IEasyRedisCache redis
            , IEasyHybirdCache hybird)
        {
            _memory = memory;
            _redis = redis;
            _hybird = hybird;
        }

        [HttpGet]
        public async Task<string> MemoryGet([FromQuery] string key)
        {
            return _memory.Get<string>(key);
        }

        [HttpGet]
        public async Task MemorySet([FromQuery] string key, [FromQuery] string value)
        {
            _memory.SetOrUpdate(key, value);
        }

        [HttpGet]
        public async Task<string> RedisGet([FromQuery] string key)
        {
            return _redis.Get<string>(key);
        }

        [HttpGet]
        public async Task RedisSet([FromQuery] string key, [FromQuery] string value)
        {
            _redis.SetOrUpdate(key, value);
        }

        [HttpGet]
        public async Task<string> HybirdGet([FromQuery] string key)
        {
            return _hybird.Get<string>(key);
        }

        [HttpGet]
        public async Task HybirdSet([FromQuery] string key, [FromQuery] string value)
        {
            _hybird.SetOrUpdate(key, value);
        }
    }
}
