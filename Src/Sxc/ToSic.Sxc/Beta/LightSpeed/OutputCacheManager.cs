using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Beta.LightSpeed
{
    [PrivateApi]
    public class OutputCacheManager
    {
        internal const string GlobalCacheRoot = "2sxc.Lightspeed.";

        public bool IsEnabled => true;

        public bool HasCache(int moduleId)
        {
            return IsEnabled && CachedIds.ContainsKey(moduleId);
        }

        public void Add(int moduleId, OutputCacheItem data)
        {
            var expiration = new TimeSpan(0, 0, 30);
            var policy = new CacheItemPolicy { SlidingExpiration = expiration };
            //var data = new OutputCacheItem { Html = item };
            Cache.Add(new CacheItem(GlobalCacheRoot + moduleId, data), policy);
            CachedIds[moduleId] = true;
        }

        public OutputCacheItem Get(int moduleId) => Cache[GlobalCacheRoot + moduleId] as OutputCacheItem;

        private static Dictionary<int, bool> CachedIds = new Dictionary<int, bool>();

        private static ObjectCache Cache => MemoryCache.Default;

    }
}
