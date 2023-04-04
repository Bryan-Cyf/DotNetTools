using EasyCaching.Core;
using System;

namespace Tools.Cache
{
    public class HybirdEasyManager : EasyHybirdBaseManager, IEasyHybirdCache
    {
        public override string Name => CacheConst.Hybird;

        public HybirdEasyManager(IHybridProviderFactory factory) : base(factory)
        {

        }
    }
}
