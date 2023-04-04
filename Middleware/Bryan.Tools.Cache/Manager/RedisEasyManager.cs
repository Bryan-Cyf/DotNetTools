using EasyCaching.Core;
using System;

namespace Tools.Cache
{
    public class RedisEasyManager : EasyRedisBaseManager, IEasyRedisCache
    {
        public override string Name => CacheConst.CsRedis;

        public RedisEasyManager(IEasyCachingProviderFactory factory) : base(factory)
        {

        }
    }
}
