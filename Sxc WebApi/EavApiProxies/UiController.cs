using System.Web.Http.Controllers;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)] // while in dev-mode, only for super-users
    [ValidateAntiForgeryToken]
    public partial class UiController : SxcApiControllerBase
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext); // very important!!!
            Log.Rename("Api.UiCont");
        }
        
    }
}
