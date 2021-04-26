using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Controllers.Assets
{
    // Release routes
    [Route(WebApiConstants.AppRoot + "/{appName}/assets")]
    [Route(WebApiConstants.AppRoot2 + "/{appName}/assets")]
    [Route(WebApiConstants.AppRoot3 + "/{appName}/assets")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/assets/{appName}")]
    public class AppAssetsController: ToSic.Sxc.Oqt.Server.Controllers.AppAssetsController
    {
        public override string Route => "assets";

        public AppAssetsController(AppAssetsDependencies dependencies) : base(dependencies)
        { }
    }
}
