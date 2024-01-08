using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Code.Internal
{
    [PrivateApi]
    public class AssemblyCacheManager() : ServiceBase(Constants.SxcLogName + ".AssemblyCacheManager")
    {
        private const string GlobalCacheRoot = "2sxc.AssemblyCache.Module.";

        #region Static Calls for AppCode - to use before requiring DI

        public static (AssemblyResult Result, string cacheKey) TryGetThisAppCode(int appId)
        {
            var cacheKey = KeyAppCode(appId);
            return (Get(cacheKey), cacheKey);
        }

        private static string KeyAppCode(int appId) => $"{GlobalCacheRoot}a:{appId}.App.Code";

        #endregion

        #region Static Calls Only - for use before the object is created using DI

        private static string KeyTemplate(string templateFullPath) => $"{GlobalCacheRoot}v:{templateFullPath.ToLowerInvariant()}";

        private static AssemblyResult Get(string key) => Cache[key] as AssemblyResult;

        public static (AssemblyResult Result, string cacheKey) TryGetTemplate(string templateFullPath)
        {
            var cacheKey = KeyTemplate(templateFullPath);
            return (Get(cacheKey), cacheKey);
        }

        #endregion

        public string Add(string cacheKey, AssemblyResult data, int duration = 3600, IList<ChangeMonitor> changeMonitor = null, CacheEntryUpdateCallback updateCallback = null)
        {
            var l = Log.Fn<string>($"{nameof(cacheKey)}: {cacheKey}; {nameof(duration)}: {duration}", timer: true);

            // Never store 0, that's like never-expire
            if (duration == 0) duration = 1;
            var expiration = new TimeSpan(0, 0, duration);
            var policy = new CacheItemPolicy { SlidingExpiration = expiration };

            // Try set app change folder monitor
            if (changeMonitor?.Any() == true)
                try
                {
                    l.Do(message: $"add {nameof(changeMonitor)}", timer: true, action: () =>
                    {
                        foreach (var changeMon in changeMonitor)
                            policy.ChangeMonitors.Add(changeMon);
                    });
                }
                catch (Exception ex)
                {
                    l.E("Error during set app folder ChangeMonitor");
                    l.Ex(ex);
                    /* ignore for now */
                    return l.ReturnAsError("error");
                }

            // Register Callback - usually to remove something from another cache
            if (updateCallback != null)
                policy.UpdateCallback = updateCallback;

            // Try to add to cache
            try
            {
                l.Do(message: $"cache set cacheKey:{cacheKey}", timer: true,
                    action: () => Cache.Set(new CacheItem(cacheKey, data), policy));

                return l.ReturnAsOk(cacheKey);
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                /* ignore for now */
                return l.ReturnAsError("error");
            }
        }

        private static MemoryCache Cache => MemoryCache.Default;
    }
}
