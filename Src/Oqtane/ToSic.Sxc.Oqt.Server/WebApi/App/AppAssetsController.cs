using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.WebApi.App
{
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)] // use view, all methods must re-check permissions
    //[Authorize(Roles = RoleNames.Everyone)] commented because of http403 issue
    // TODO: 2DM please check permissions

    // Release routes
    [Route(WebApiConstants.AppRoot + "/{appName}/assets")]
    [Route(WebApiConstants.AppRoot2 + "/{appName}/assets")]
    [Route(WebApiConstants.AppRoot3 + "/{appName}/assets")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/assets/{appName}")]
    public class AppAssetsController: WebApi.AppAssetsControllerBase
    {
        protected override string HistoryLogName => "Oqt.AppAst";

        public override string Route => "assets";

        public AppAssetsController(AppAssetsDependencies dependencies) : base(dependencies)
        { }

    }
}
