using System;
using System.Collections.Generic;
using System.Net;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
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
        protected PortalSettings Portal;

        public Security(PortalSettings portal, Log parentLog) 
            : base("Api.SecChk", parentLog) 
            => Portal = portal;


        /// <summary>
        /// Check if a user may do something - and throw an error if the permission is not given
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="contentType"></param>
        /// <param name="grant"></param>
        /// <param name="specificItem"></param>
        /// <param name="module"></param>
        /// <param name="zoneId">Optional zone-id, will enforce that the app must be in the current zone</param>
        internal void FindCtCheckSecurityOrThrow(int appId, string contentType, List<Grants> grant, IEntity specificItem, ModuleInfo module, /*App app, */int? zoneId)
        {
            Log.Add($"security check for type:{contentType}, grant:{grant}, useContext:{module != null}, zone/app:{zoneId}/{appId}, item:{specificItem?.EntityId}");
            // make sure we have the right appId, zoneId and module-context
            var ct = FindContentTypeOrThrow(appId, contentType, module, /*app?.ZoneId*/zoneId);

            // Check if the content-type has a GUID as name - only these can have permission assignments
            // only check permissions on type if the type has a GUID as static-id
            var staticNameIsGuid = Guid.TryParse(ct.StaticName, out var _);
            // Check permissions in 2sxc - or check if the user has admin-right (in which case he's always granted access for these types of content)
            if (new DnnPermissionCheck(Log, ct,
                    specificItem,
                    new DnnInstanceInfo(module),
                    portal: Portal
                ).UserMay(grant))
                return;

            throw Errors.Http.InformativeErrorForTypeAccessDenied(contentType, grant, staticNameIsGuid);
        }

        private static IContentType FindContentTypeOrThrow(int appId, string contentType, ModuleInfo module, int? zoneId)
        {
            
            // accessing the app for the ID only works if we have a context
            // from the module
            // this is not the case in certain API-calls, then context-access shouldn't happen
            var useContext = module != null;
            var usedZoneId = useContext ? /*app?.ZoneId*/ zoneId : null;
            // 2018-04-10 disabled this - should always check for the app provided in the request
            //if (appId <= 0 && useContext) appId = app?.AppId ?? appId;

            // Ensure that we can find this content-type 
            var cache = DataSource.GetCache(usedZoneId, appId);
            var ct = cache.GetContentType(contentType);
            if (ct == null)
                throw Errors.Http.WithLink(HttpStatusCode.NotFound, 
                    "Could not find Content Type '" + contentType + "'.",
                    "content-types");
            return ct;
        }



    }
}
