using System;
using System.Runtime.Caching;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Beta.LightSpeed
{
    [PrivateApi]
    public class OutputCacheManager
    {
        internal const string GlobalCacheRoot = "2sxc.Lightspeed.Module.";

        public bool IsEnabled => false;

        public bool HasCache(int moduleId)
        {
            return IsEnabled && Cache.Contains(Id(moduleId));
        }

        private string Id(int moduleId) => GlobalCacheRoot + moduleId;

        public void Add(int moduleId, OutputCacheItem data, int duration)
        {
            try
            {
                // Never store 0, that's like never-expire
                if (duration == 0) duration = 1;

                var expiration = new TimeSpan(0, 0, duration);
                var policy = new CacheItemPolicy { SlidingExpiration = expiration };
                Cache.Set(new CacheItem(Id(moduleId), data), policy);
            }
            catch
            {
                /* ignore for now */
            }
        }

        public OutputCacheItem Get(int moduleId) => Cache[Id(moduleId)] as OutputCacheItem;

        private static MemoryCache Cache => MemoryCache.Default;

    }
}
