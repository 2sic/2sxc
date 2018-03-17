using System;
using System.Collections.Generic;
using System.Net;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Permissions;
using ToSic.Eav;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.Environment.Dnn7;

namespace ToSic.SexyContent.WebApi
{
    internal class Security: HasLog
    {
        protected UserInfo User;

        public Security(UserInfo user, Log parentLog) : base("Api.SecChk", parentLog)
        {
            User = user;
        }


        /// <summary>
        /// Check if a user may do something - and throw an error if the permission is not given
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="contentType"></param>
        /// <param name="grant"></param>
        /// <param name="specificItem"></param>
        /// <param name="module"></param>
        /// <param name="app"></param>
        internal void PerformSecurityCheck(int appId, string contentType, List<PermissionGrant> grant, IEntity specificItem, ModuleInfo module, App app)
        {
            Log.Add($"security check for type:{contentType}, grant:{grant}, useContext:{module != null}, app:{appId}, item:{specificItem?.EntityId}");
            // make sure we have the right appId, zoneId and module-context
            ResolveModuleAndIDsOrThrow(module, app, appId, out var zoneId, out appId);

            // Ensure that we can find this content-type 
            var cache = DataSource.GetCache(zoneId, appId);
            var ct = cache.GetContentType(contentType);
            if (ct == null)
                throw Errors.Http.WithLink(HttpStatusCode.NotFound, "Could not find Content Type '" + contentType + "'.",
                    "content-types");

            // give ok for all host-users
            if (User?.IsSuperUser ?? false)
                return;

            // Check if the content-type has a GUID as name - only these can have permission assignments
            // only check permissions on type if the type has a GUID as static-id
            var staticNameIsGuid = Guid.TryParse(ct.StaticName, out var _);
            // Check permissions in 2sxc - or check if the user has admin-right (in which case he's always granted access for these types of content)
            if (staticNameIsGuid
                && new DnnPermissionCheck(Log, ct, specificItem, new DnnInstanceInfo(module))
                    .UserMay(grant))
                return;

            // if initial test couldn't be done (non-guid) or failed, test for admin-specifically
            // note that auto-check not possible when not using context
            if (module != null && ModulePermissionController.CanAdminModule(module))
                return;

            throw Errors.Http.InformativeErrorForTypeAccessDenied(contentType, grant, staticNameIsGuid);
        }





        private static void ResolveModuleAndIDsOrThrow(ModuleInfo module, App app, int? appIdOpt, out int? zoneId, out int appId)
        {
            // accessing the app for the ID only works if we have a context
            // from the module
            // this is not the case in certain API-calls, then context-access shouldn't happen
            var useContext = module != null;
            zoneId = useContext ? app?.ZoneId : null;
            if (useContext) appIdOpt = app?.AppId ?? appIdOpt;

            if (!appIdOpt.HasValue)
                throw new Exception("app id doesn't have value, and apparently didn't get it from context either");

            appId = appIdOpt.Value;
        }

    }
}
