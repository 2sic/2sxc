using DotNetNuke.Web.Api;
using ToSic.SexyContent.WebApi;

namespace ToSic.Sxc.WebApi.Cms
{
    [ValidateAntiForgeryToken]
    // cannot use this, as most requests now come from a lone page [SupportedModules("2sxc,2sxc-app")]
    public class ModuleController : SxcApiController
    {

    }
}