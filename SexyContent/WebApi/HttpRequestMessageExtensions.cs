using System.Net.Http;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.Statics;

namespace ToSic.SexyContent.WebApi
{
    public static class HttpRequestMessageExtensions
    {
        internal static SexyContent GetSxcOfModuleContext(this HttpRequestMessage request, int? appId = null)
        {
            var moduleInfo = request.FindModuleInfo();
            if(!appId.HasValue)
                appId = AppHelpers.GetAppIdFromModule(moduleInfo);
            var zoneId = ZoneHelpers.GetZoneID(moduleInfo.PortalID);
            return new SexyContent(zoneId.Value, appId.Value, true, moduleInfo.OwnerPortalID, moduleInfo);
        }

    }
}
