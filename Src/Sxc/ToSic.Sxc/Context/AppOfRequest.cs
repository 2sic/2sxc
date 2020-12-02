using System;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// This helps API calls to get the app which is currently needed
    /// It does not perform security checks ATM and maybe never will
    /// </summary>
    public class AppIdResolver: HasLog<AppIdResolver>
    {
        #region Constructor & DI

        private readonly IHttp _http;

        public AppIdResolver(IHttp http): base("Api.FindAp")
        {
            _http = http;
        }

        #endregion

        ///// <summary>
        ///// find the AppIdentity of an app which is referenced by a path
        ///// </summary>
        ///// <param name="appPath"></param>
        ///// <returns></returns>
        //internal IAppIdentity GetAppIdFromPath(string appPath)
        //{
        //    var wrapLog = Log.Call(appPath);
        //    var zid = _zoneMapper.GetZoneId(_siteId);

        //    // get app from AppName
        //    var aid = new ZoneRuntime().Init(zid, Log).FindAppId(appPath, true);
        //    wrapLog($"found app:{aid}");
        //    return new AppIdentity(zid, aid);
        //}

        /// <summary>
        /// New implementation to replace previous
        /// </summary>
        /// <returns></returns>
        internal int GetAppIdFromPath(int zoneId, string appPath, bool required)
        {
            var wrapLog = Log.Call<int>($"{zoneId}, {appPath}, {required}");
            // get app from AppName
            var aid = new ZoneRuntime().Init(zoneId, Log).FindAppId(appPath, true);
            if (aid <= Eav.Constants.AppIdEmpty && required)
                throw new Exception($"App required but can't find App based on the name '{appPath}'");

            return wrapLog($"found app:{aid}", aid);
        }



        ///// <summary>
        ///// Retrieve the appId - either based on the parameter, or if missing, use context
        ///// Note that this will fail, if both appPath and context are missing
        ///// </summary>
        ///// <returns></returns>
        //internal IAppIdentity GetAppIdFromPathOrContext(string appPath, IContextOfApp context)
        //{
        //    var wrapLog = Log.Call<IAppIdentity>($"{appPath}, ...", message: "detect app from query string parameters");

        //    // try to override detection based on additional zone/app-id in urls
        //    var appId = GetAppIdFromRoute();

        //    if (appId != null) return wrapLog(appId.LogState(), appId);


        //    Log.Add($"auto detect app and init eav - path:{appPath}, context null: {context == null}");
        //    appId = appPath == null || appPath == "auto"
        //        ? context?.AppState ?? throw new ArgumentException("try to get app-id from context, but none found")
        //        : GetAppIdFromPath(appPath);

        //    return wrapLog(appId.LogState(), appId);
        //}

        /// <summary>
        /// This will detect the app based on appid/zoneid params in the URL
        /// It's a temporary solution, because normally we want the control flow to be more obvious
        /// </summary>
        /// <returns></returns>
        internal IAppIdentity GetAppIdFromRoute()
        {
            var allUrlKeyValues = _http.QueryStringKeyValuePairs();
            var ok1 = int.TryParse(allUrlKeyValues.FirstOrDefault(
                x => x.Key.Equals(ContextConstants.ZoneIdKey, StringComparison.InvariantCultureIgnoreCase)).Value, 
                out var zoneId);
            var ok2 = int.TryParse(allUrlKeyValues.FirstOrDefault(
                x => x.Key.Equals(ContextConstants.AppIdKey, StringComparison.InvariantCultureIgnoreCase)).Value, 
                out var appId);

            if (!ok1 || !ok2) return null;

            Log.Add($"Params in URL detected - will use appId:{appId}, zoneId:{zoneId}");
            return new AppIdentity(zoneId, appId);
        }

    }
}
