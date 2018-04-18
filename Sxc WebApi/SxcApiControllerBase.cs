using System.Collections.Generic;
using System.Web.Http.Controllers;
using DotNetNuke.Entities.Modules;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Internal;
using ToSic.SexyContent.WebApi.Dnn;

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

        protected static DnnAppAndDataHelpers GetContext(SxcInstance sxcInstance, Log log) => new DnnAppAndDataHelpers(sxcInstance, sxcInstance?.Log ?? log);

        #region Security Checks 

        /// <summary>
        /// Check if a user may do something - and throw an error if the permission is not given
        /// </summary>
        internal void PerformSecurityCheck(int appId, string contentType, Grants grant,
            ModuleInfo module, int? zoneId, IEntity specificItem = null)
            => new Security(PortalSettings, Log).FindCtCheckSecurityOrThrow(appId,
                contentType,
                new List<Grants> { grant },
                specificItem,
                module, 
                zoneId);

        #endregion

        #region App-Helpers for anonyous access APIs

        internal int GetCurrentAppIdFromPath(string appPath)
        {
            // check zone
            var zid = Env.ZoneMapper.GetZoneId(PortalSettings.PortalId);

            // get app from appname
            var aid = AppHelpers.GetAppIdFromGuidName(zid, appPath, true);
            Log.Add($"find app by path:{appPath}, found a:{aid}");
            return aid;
        }
        #endregion
    }
}
