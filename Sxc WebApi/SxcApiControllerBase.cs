using DotNetNuke.Entities.Modules;
using System.Collections.Generic;
using System.Web.Http.Controllers;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Internal;
using ToSic.SexyContent.WebApi.Dnn;
using ToSic.SexyContent.WebApi.Permissions;

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
        

        internal static DnnAppAndDataHelpers GetContext(SxcInstance sxcInstance, Log log) 
            => new DnnAppAndDataHelpers(sxcInstance, sxcInstance?.Log ?? log);



        #region Security Checks 

        internal void PerformSecurityCheck(IAppIdentity appIdentity, string contentType,
            Grants grant, ModuleInfo module, IEntity specificItem = null)
            => new AppPermissionBeforeUsing(SxcInstance, Log)
                .PerformSecurityCheck(PortalSettings, appIdentity, contentType, grant, module, specificItem);



        #endregion

        #region App-Helpers for anonyous access APIs

        /// <summary>
        /// find the AppIdentity of an app which is referenced by a path
        /// </summary>
        /// <param name="appPath"></param>
        /// <returns></returns>
        internal IAppIdentity GetCurrentAppIdFromPath(string appPath)
        {
            // check zone
            var zid = Env.ZoneMapper.GetZoneId(PortalSettings.PortalId);

            // get app from appname
            var aid = AppHelpers.GetAppIdFromGuidName(zid, appPath, true);
            Log.Add($"find app by path:{appPath}, found a:{aid}");
            return new AppIdentity(zid, aid, Log);
        }
        #endregion
    }
}
