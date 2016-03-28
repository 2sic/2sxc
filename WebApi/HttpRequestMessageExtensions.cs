using System.Net.Http;
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Web.Api;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.WebApi
{
    public static class HttpRequestMessageExtensions
    {
        internal static SxcInstance GetSxcOfModuleContext(this HttpRequestMessage request, int? appId = null)
        {
            var moduleInfo = request.FindModuleInfo();
            if(!appId.HasValue)
                appId = AppHelpers.GetAppIdFromModule(moduleInfo);
            var zoneId = ZoneHelpers.GetZoneID(moduleInfo.PortalID);
            var Req = HttpContext.Current.Request;
            var contentBlock = new ModuleContentBlock(moduleInfo, PortalSettings.Current.UserInfo, Req.QueryString);
            return contentBlock.SxcInstance;// new SxcInstance(zoneId.Value, appId.Value, moduleInfo);
        }

    }
}
