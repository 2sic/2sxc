using System.Net.Http;
using DotNetNuke.Web.Api;

namespace ToSic.SexyContent.WebApi
{
    public static class HttpRequestMessageExtensions
    {
        internal static SexyContent GetSxcOfModuleContext(this HttpRequestMessage request, int? appId = null)
        {
            var moduleInfo = request.FindModuleInfo();
            if(!appId.HasValue)
                appId = SexyContent.GetAppIdFromModule(moduleInfo);
            var zoneId = SexyContent.GetZoneID(moduleInfo.PortalID);
            return new SexyContent(zoneId.Value, appId.Value, true, moduleInfo.OwnerPortalID, moduleInfo);
        }

    }
}
