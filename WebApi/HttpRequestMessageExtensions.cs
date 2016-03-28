using System.Net.Http;
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Web.Api;

namespace ToSic.SexyContent.WebApi
{
    public static class HttpRequestMessageExtensions
    {
        internal static SxcInstance GetSxcOfModuleContext(this HttpRequestMessage request)
        {
            var moduleInfo = request.FindModuleInfo();
            var contentBlock = new ModuleContentBlock(moduleInfo);
            return contentBlock.SxcInstance;
        }

    }
}
