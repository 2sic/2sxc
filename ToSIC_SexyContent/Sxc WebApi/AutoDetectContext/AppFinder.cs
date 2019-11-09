using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Internal;

namespace ToSic.SexyContent.WebApi.AutoDetectContext
{
    /// <summary>
    /// This helps API calls to get the app which is currently needed
    /// It does not perform security checks ATM and maybe never will
    /// </summary>
    internal class AppFinder: HasLog
    {
        private IZoneMapper ZoneMapper { get; }
        private PortalSettings Portal { get; }
        private HttpControllerContext ControllerContext { get; }

        public AppFinder(PortalSettings portal, IZoneMapper zoneMapper, HttpControllerContext controllerContext, ILog parentLog) : base("Api.FindAp", parentLog, null, "AppForApiCall")
        {
            ZoneMapper = zoneMapper;
            Portal = portal;
            ControllerContext = controllerContext;
        }

        /// <summary>
        /// find the AppIdentity of an app which is referenced by a path
        /// </summary>
        /// <param name="appPath"></param>
        /// <returns></returns>
        internal IAppIdentity GetCurrentAppIdFromPath(string appPath)
        {
            var wrapLog = Log.Call("GetCurrentAppIdFromPath", appPath);
            // check zone
            var zid = ZoneMapper.GetZoneId(Portal.PortalId);

            // get app from appname
            var aid = AppHelpers.GetAppIdFromGuidName(zid, appPath, true);
            wrapLog($"found app:{aid}");
            return new AppIdentity(zid, aid, Log);
        }


        /// <summary>
        /// Retrieve the appId - either based on the parameter, or if missing, use context
        /// Note that this will fail, if both appPath and context are missing
        /// </summary>
        /// <returns></returns>
        internal IAppIdentity GetAppIdFromPathOrContext(string appPath, /*SxcBlock*/ICmsBlock cmsInstance)
        {
            var wrapLog = Log.Call("GetAppIdFromPathOrContext", $"{appPath}, ...", "detect app from query string parameters");

            // try to override detection based on additional zone/app-id in urls
            var appId = GetAppIdentityFromQueryAppZone();

            if (appId == null)
            {
                Log.Add($"auto detect app and init eav - path:{appPath}, context null: {cmsInstance == null}");
                appId = appPath == null || appPath == "auto"
                    ? new AppIdentity(
                        cmsInstance?.ZoneId ??
                        throw new ArgumentException("try to get app-id from context, but none found"),
                        cmsInstance.AppId // not nullable any more 2019-11-09 ?? 0
                        , Log)
                    : GetCurrentAppIdFromPath(appPath);
            }

            wrapLog(appId.LogState());

            return appId;
        }

        /// <summary>
        /// This will detect the app based on appid/zoneid params in the URL
        /// It's a temporary solution, because normally we want the control flow to be more obvious
        /// </summary>
        /// <returns></returns>
        private IAppIdentity GetAppIdentityFromQueryAppZone()
        {
            var allUrlKeyValues = ControllerContext.Request.GetQueryNameValuePairs().ToList();
            var ok1 = int.TryParse(allUrlKeyValues.FirstOrDefault(x => x.Key == Route.ZoneIdKey).Value, out var zoneIdFromQueryString);
            var ok2 = int.TryParse(allUrlKeyValues.FirstOrDefault(x => x.Key == Route.AppIdKey).Value, out var appIdFromQueryString);
            if (ok1 && ok2)
            {
                Log.Add($"Params in URL detected - will use appId:{appIdFromQueryString}, zoneId:{zoneIdFromQueryString}");
                return new AppIdentity(zoneIdFromQueryString, appIdFromQueryString, Log);
            }
            return null;
        }

    }
}
