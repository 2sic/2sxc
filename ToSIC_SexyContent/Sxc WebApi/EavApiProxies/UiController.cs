using System.Web.Http.Controllers;
using DotNetNuke.Web.Api;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    [SupportedModules("2sxc,2sxc-app")]
    // 2019-09-18 2dm enabled [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)] // while in dev-mode, only for super-users
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
