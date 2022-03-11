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

        public bool IsEnabled => false;

        public bool HasCache(int moduleId)
        {
            return IsEnabled && CachedIds.ContainsKey(moduleId);
        }

        public void Add(int moduleId, OutputCacheItem data)
        {
            try
            {
                var expiration = new TimeSpan(0, 0, 30);
                var policy = new CacheItemPolicy { SlidingExpiration = expiration };
                Cache.Add(new CacheItem(GlobalCacheRoot + moduleId, data), policy);
                // Note: we've had someone report that this threw an index-out-of-bounds
                // https://stackoverflow.com/questions/71329719/2sxc-index-was-outside-the-bounds-of-the-array
                // Can't imagine how, but must wrap in try catch for now
                CachedIds[moduleId] = true;
            }
            catch
            {
                /* ignore for now */
            }
        }

        public OutputCacheItem Get(int moduleId) => Cache[GlobalCacheRoot + moduleId] as OutputCacheItem;

        private static Dictionary<int, bool> CachedIds = new Dictionary<int, bool>();

        private static ObjectCache Cache => MemoryCache.Default;

    }
}
