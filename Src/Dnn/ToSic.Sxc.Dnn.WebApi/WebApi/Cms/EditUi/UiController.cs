using DotNetNuke.Web.Api;

namespace ToSic.Sxc.WebApi.Cms
{
    [SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
    public partial class UiController : SxcApiControllerBase
    {
        protected override string HistoryLogName => "Api.UiCont";

        //protected override void Initialize(HttpControllerContext controllerContext)
        //{
        //    base.Initialize(controllerContext); // very important!!!
        //    Log.Rename("Api.UiCont");
        //}
        
    }
}
