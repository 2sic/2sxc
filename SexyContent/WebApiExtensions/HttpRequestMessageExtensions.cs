using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Web.Api;
using System.Net.Http;

namespace ToSic.SexyContent.WebApiExtensions
{
    public static class HttpRequestMessageExtensions
    {
        internal static SexyContent GetSxcOfModuleContext(this HttpRequestMessage request)
        {
            var moduleInfo = request.FindModuleInfo();
            var appId = SexyContent.GetAppIdFromModule(moduleInfo);
            var zoneId = SexyContent.GetZoneID(moduleInfo.PortalID);
            return new SexyContent(zoneId.Value, appId.Value, true, moduleInfo.OwnerPortalID);
        }

        internal static SexyContent GetUncachedSxcOfModuleContext(this HttpRequestMessage request)
        {
            var moduleInfo = request.FindModuleInfo();
            var appId = SexyContent.GetAppIdFromModule(moduleInfo);
            var zoneId = SexyContent.GetZoneID(moduleInfo.PortalID);
            return new SexyContent(zoneId.Value, appId.Value, false, moduleInfo.OwnerPortalID);
        }

    }
}
