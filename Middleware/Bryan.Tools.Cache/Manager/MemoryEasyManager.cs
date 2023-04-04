using EasyCaching.Core;
using System;

namespace Tools.Cache
{
    public class MemoryEasyManager : EasyBaseManager, IEasyMemoryCache
    {
        public override string Name => CacheConst.Memmory;

        public MemoryEasyManager(IEasyCachingProviderFactory factory) : base(factory)
        {

        }

    }
}
