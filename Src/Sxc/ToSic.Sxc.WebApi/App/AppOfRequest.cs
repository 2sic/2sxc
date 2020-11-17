using System;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.WebApi.App
{
    /// <summary>
    /// This helps API calls to get the app which is currently needed
    /// It does not perform security checks ATM and maybe never will
    /// </summary>
    internal class AppOfRequest: HasLog
    {
        #region Constructor & DI 

        private readonly IZoneMapper _zoneMapper;
        private readonly int _siteId;
        private readonly IHttp _http;

        public AppOfRequest(IHttp http, ISite site, IZoneMapper zoneMapper): base("Api.FindAp")
        {
            _http = http;
            _siteId = site.Id;
            _zoneMapper = zoneMapper;
        }

        public AppOfRequest Init(ILog parentLog) 
        {
            Log.LinkTo(parentLog);
            _zoneMapper.Init(Log);
            return this;
        }
        #endregion

        /// <summary>
        /// find the AppIdentity of an app which is referenced by a path
        /// </summary>
        /// <param name="appPath"></param>
        /// <returns></returns>
        internal IAppIdentity GetAppIdFromPath(string appPath)
        {
            var wrapLog = Log.Call(appPath);
            var zid = _zoneMapper.GetZoneId(_siteId);

            // get app from AppName
            var aid = new ZoneRuntime().Init(zid, Log).FindAppId(appPath, true);
            wrapLog($"found app:{aid}");
            return new AppIdentity(zid, aid);
        }


        /// <summary>
        /// Retrieve the appId - either based on the parameter, or if missing, use context
        /// Note that this will fail, if both appPath and context are missing
        /// </summary>
        /// <returns></returns>
        internal IAppIdentity GetAppIdFromPathOrContext(string appPath, IBlock block)
        {
            var wrapLog = Log.Call<IAppIdentity>($"{appPath}, ...", message: "detect app from query string parameters");

            // try to override detection based on additional zone/app-id in urls
            var appId = GetAppIdFromUrlParams();

            if (appId != null) return wrapLog(appId.LogState(), appId);


            Log.Add($"auto detect app and init eav - path:{appPath}, context null: {block == null}");
            appId = appPath == null || appPath == "auto"
                ? new AppIdentity(
                    block?.ZoneId ??
                    throw new ArgumentException("try to get app-id from context, but none found"),
                    block.AppId)
                : GetAppIdFromPath(appPath);

            return wrapLog(appId.LogState(), appId);
        }

        /// <summary>
        /// This will detect the app based on appid/zoneid params in the URL
        /// It's a temporary solution, because normally we want the control flow to be more obvious
        /// </summary>
        /// <returns></returns>
        private IAppIdentity GetAppIdFromUrlParams()
        {
            var allUrlKeyValues = _http.QueryStringKeyValuePairs();
            var ok1 = int.TryParse(allUrlKeyValues.FirstOrDefault(
                x => x.Key.Equals(WebApiConstants.ZoneIdKey, StringComparison.InvariantCultureIgnoreCase)).Value, 
                out var zoneId);
            var ok2 = int.TryParse(allUrlKeyValues.FirstOrDefault(
                x => x.Key.Equals(WebApiConstants.AppIdKey, StringComparison.InvariantCultureIgnoreCase)).Value, 
                out var appId);

            if (!ok1 || !ok2) return null;

            Log.Add($"Params in URL detected - will use appId:{appId}, zoneId:{zoneId}");
            return new AppIdentity(zoneId, appId);
        }

    }
}
