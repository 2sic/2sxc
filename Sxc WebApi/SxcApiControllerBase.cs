using DotNetNuke.Entities.Modules;
using System.Collections.Generic;
using System.Web.Http.Controllers;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Internal;
using ToSic.SexyContent.WebApi.Dnn;
using ToSic.SexyContent.WebApi.Errors;

namespace ToSic.SexyContent.WebApi
{
    /// <summary>
    /// This class is the base class of 2sxc API access
    /// It will auto-detect the SxcInstance context
    /// But it will NOT provide an App or anything like that
    /// </summary>
    [SxcWebApiExceptionHandling]
    public class SxcApiControllerBase: DnnApiControllerWithFixes
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            Log.Rename("Api.CntBas");
            SxcInstance = Helpers.GetSxcOfApiRequest(Request, true, Log);
        }

        internal SxcInstance SxcInstance { get; private set; }
        

        protected static DnnAppAndDataHelpers GetContext(SxcInstance sxcInstance, Log log) 
            => new DnnAppAndDataHelpers(sxcInstance, sxcInstance?.Log ?? log);

        protected static IAppIdentity GetAppIdentity(DnnAppAndDataHelpers context, int appId, bool superUser)
        {
            IAppIdentity appIdentity;

            if (context.App.AppId == appId)
                appIdentity = context.App;
            else
            {
                // check if app is in current zone - allow switching zones for host users
                var appRun = new AppRuntime(appId, context.Log);
                if (appRun.ZoneId == context.App.ZoneId)
                    appIdentity = appRun;
                else if (superUser)
                    appIdentity = appRun;
                else
                    throw Http.PermissionDenied(
                        $"accessing app {appId} in zone {appRun.ZoneId} is not allowed for this user");
            }
            return appIdentity;
        }

        #region Security Checks 

        /// <summary>
        /// Check if a user may do something - and throw an error if the permission is not given
        /// </summary>
        internal void PerformSecurityCheck(IAppIdentity zaId, /*int appId,*/ string contentType,
            Grants grant, ModuleInfo module, IEntity specificItem = null)
            => new Security(PortalSettings, Log).FindCtCheckSecurityOrThrow(zaId,
                //appId,
                contentType,
                new List<Grants> { grant },
                specificItem,
                module);

        #endregion

        #region App-Helpers for anonyous access APIs

        internal IAppIdentity GetCurrentAppIdFromPath(string appPath)
        {
            // check zone
            var zid = Env.ZoneMapper.GetZoneId(PortalSettings.PortalId);

            // get app from appname
            var aid = AppHelpers.GetAppIdFromGuidName(zid, appPath, true);
            Log.Add($"find app by path:{appPath}, found a:{aid}");
            return new AppIdentity(zid, aid);
        }
        #endregion
    }
}
