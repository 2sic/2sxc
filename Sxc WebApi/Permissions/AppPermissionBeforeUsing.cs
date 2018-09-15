using System;
using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.WebApi.Errors;

namespace ToSic.SexyContent.WebApi.Permissions
{
    internal class AppPermissionBeforeUsing: HasLog
    {
        private readonly SxcInstance _sxcInstance;

        public AppPermissionBeforeUsing(SxcInstance sxcInstance, Log parentLog) : base("App.PrePrm", parentLog)
        {
            _sxcInstance = sxcInstance;
        }

        internal void ConfirmPermissionsOrThrow(string contentType, int appId, List<Grants> grants)
        {
            var wrapLog = Log.New("ConfirmPermissionsOrThrow", () => $"{contentType}, {appId}, [{string.Join(",", grants)}]");
            var context = SxcApiControllerBase.GetContext(_sxcInstance, Log);
            GetAppIdentityOrThrowIfNotAllowed(context, appId);

            var permCheck = new PermissionsForAppAndTypes(_sxcInstance, appId, contentType, Log);
            if (!permCheck.Ensure(grants, /*contentType,*/ out var exp))
                throw exp;
            wrapLog("ok");
        }

        [Obsolete("should use the other one instead, with the grants-list, as it also looks in the app permissions etc.")]
        internal void ConfirmPermissionsOrThrow(string contentType, int appId, Grants grant)
        {
            // check if admin rights, then ok
            var context = SxcApiControllerBase.GetContext(_sxcInstance, Log);
            var zaId = GetAppIdentityOrThrowIfNotAllowed(context, appId);
            PerformSecurityCheck(context.Dnn.Portal, zaId, contentType, grant, context.Dnn.Module);
        }

        /// <summary>
        /// Check if a user may do something - and throw an error if the permission is not given
        /// </summary>
        internal void PerformSecurityCheck(PortalSettings portalSettings, IAppIdentity appIdentity, string contentType,
            Grants grant, ModuleInfo module, IEntity specificItem = null)
            => new Security(portalSettings, Log)
                .FindCtCheckSecurityOrThrow(appIdentity, contentType, new List<Grants> { grant },
                    specificItem, module);

        /// <summary>
        /// Retrieve an AppIdentity object for an app, based on the context and alternate appId
        /// Will only allow apps outside of current zone if it's a superuser
        /// </summary>
        /// <returns></returns>
        public IAppIdentity GetAppIdentityOrThrowIfNotAllowed(DnnAppAndDataHelpers context, int appId)
        {
            var wrapLog = Log.Call("GetAppIdentityOrThrowIfNotAllowed", $"..., app: {appId}");
            IAppIdentity appIdentity;
            var superUser = context.Dnn.Portal.UserInfo.IsSuperUser;

            if (context.App.AppId == appId)
                appIdentity = context.App;
            else
            {
                // check if app is in current zone - allow switching zones for host users
                var appRun = new AppRuntime(appId, context.Log);
                appIdentity = ThrowIfZoneChangeWithoutSuperUser(context, appRun, superUser);
            }
            wrapLog("ok");
            return appIdentity;
        }

        private IAppIdentity ThrowIfZoneChangeWithoutSuperUser(DnnAppAndDataHelpers context,
            IAppIdentity appRun, bool superUser)
        {
            var wrapLog = Log.Call("ThrowIfZoneChangeWithoutSuperUser");
            IAppIdentity appIdentity;
            if (appRun.ZoneId == context.App.ZoneId || superUser)
                appIdentity = appRun;
            else
                throw Http.PermissionDenied(
                    $"accessing app {appRun.AppId} in zone {appRun.ZoneId} is not allowed for this user");
            wrapLog("ok");
            return appIdentity;
        }

        /// <summary>
        /// Retrieve an AppIdentity object for an app, based on the context and alternate appId
        /// Will only allow apps outside of current zone if it's a superuser
        /// </summary>
        /// <returns></returns>
        public IAppIdentity GetAppIdentityOrThrowIfNotAllowed(int appId)
            => GetAppIdentityOrThrowIfNotAllowed(SxcApiControllerBase.GetContext(_sxcInstance, Log), appId);


    }
}
