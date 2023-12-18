using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using ToSic.Eav.Caching.CachingMonitors;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Code
{
    [PrivateApi]
    public class AssemblyCacheManager() : ServiceBase(Constants.SxcLogName + ".AssemblyCacheManager")
    {
        private const string GlobalCacheRoot = "2sxc.AssemblyCache.Module.";

        #region Static Calls Only - for use before the object is created using DI

        public static string KeyAppCode(int appId) => $"{GlobalCacheRoot}a:{appId}.App.Code";
        public static string KeyTemplate(string templateFullPath) => $"{GlobalCacheRoot}v:{templateFullPath}";

        public static bool Has(string key) => Cache.Contains(key);
        public static bool HasAppCode(int appId) => Cache.Contains(KeyAppCode(appId));
        public static bool HasTemplate(string templateFullPath) => Cache.Contains(KeyTemplate(templateFullPath));

        public static AssemblyResult Get(string key) => Cache[key] as AssemblyResult;
        public static AssemblyResult GetMyAppCode(int appId) => Get(KeyAppCode(appId));
        public static AssemblyResult GetTemplate(string templateFullPath) => Get(KeyTemplate(templateFullPath));        

        #endregion



        public string Add(string cacheKey, AssemblyResult data, int duration = 3600, IList<string> appPaths = null)
            => Add(cacheKey, data, duration, appPaths, null);

        public string Add(string cacheKey, AssemblyResult data, int duration = 3600, IList<string> appPaths = null, CacheEntryUpdateCallback updateCallback = null)
        {
            var l = Log.Fn<string>($"key: {cacheKey}", timer: true);
            try
            {
                // Never store 0, that's like never-expire
                if (duration == 0) duration = 1;
                var expiration = new TimeSpan(0, 0, duration);
                var policy = new CacheItemPolicy { SlidingExpiration = expiration };

                if (appPaths != null && appPaths.Count > 0)
                    l.Do(message: "changeMonitors add FolderChangeMonitor", timer: true, action: () =>
                        policy.ChangeMonitors.Add(new FolderChangeMonitor(appPaths)));

                if (updateCallback != null)
                    policy.UpdateCallback = updateCallback;

                l.Do(message: $"cache set cacheKey:{cacheKey}", timer: true, action: () => 
                    Cache.Set(new CacheItem(cacheKey, data), policy));

                return l.ReturnAsOk(cacheKey);
            }
            catch
            {
                /* ignore for now */
            }
            return l.ReturnAsError("error");
        }

        private static MemoryCache Cache => MemoryCache.Default;
    }
}
