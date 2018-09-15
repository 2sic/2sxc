using System;
using System.Collections.Generic;
using System.Net;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.Environment.Dnn7;

namespace ToSic.SexyContent.WebApi
{
    internal class Security: HasLog
    {
        protected PortalSettings Portal;

        public Security(PortalSettings portal, Log parentLog) 
            : base("Api.SecChk", parentLog, null, "Security") 
            => Portal = portal;


        /// <summary>
        /// Check if a user may do something - and throw an error if the permission is not given
        /// </summary>
        /// <param name="appIdentity">Optional zone-id, will enforce that the app must be in the current zone</param>
        /// <param name="contentType"></param>
        /// <param name="grant"></param>
        /// <param name="specificItem"></param>
        /// <param name="module"></param>
        internal void FindCtCheckSecurityOrThrow(IAppIdentity appIdentity, 
            string contentType, 
            List<Grants> grant,
            IEntity specificItem,
            ModuleInfo module)
        {
            Log.Add(() => $"security check for type:{contentType}, grants:[{string.Join(",", grant)}], useContext:{module != null}, zone/app:{appIdentity.ZoneId}/{appIdentity.AppId}, item:{specificItem?.EntityId}");
            // make sure we have the right appId, zoneId and module-context
            var ct = FindContentTypeOrThrow(appIdentity.ZoneId, appIdentity.AppId, contentType);

            // Check permissions in 2sxc - or check if the user has admin-right (in which case he's always granted access for these types of content)
            if (new DnnPermissionCheck(Log, 
                    ct,
                    specificItem,
                    new DnnInstanceInfo(module),
                    portal: Portal,
                    appIdentity: appIdentity
                ).UserMay(grant))
                return;

            var staticNameIsGuid = Guid.TryParse(ct.StaticName, out var _);
            throw Errors.Http.InformativeErrorForTypeAccessDenied(contentType, grant, staticNameIsGuid);
        }

        private static IContentType FindContentTypeOrThrow(int? zoneId, int appId, string contentType)
        {
            // Ensure that we can find this content-type 
            var cache = DataSource.GetCache(zoneId, appId);
            var ct = cache.GetContentType(contentType);
            if (ct == null)
                throw Errors.Http.WithLink(HttpStatusCode.NotFound, 
                    "Could not find Content Type '" + contentType + "'.",
                    "content-types");
            return ct;
        }



    }
}
