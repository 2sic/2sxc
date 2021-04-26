using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Controllers.Assets
{
    // Release routes
    [Route(WebApiConstants.ApiRoot + "/assets/{appName}")]
    [Route(WebApiConstants.ApiRoot2 + "/assets/{appName}")]
    [Route(WebApiConstants.ApiRoot3 + "/assets/{appName}")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/assets/{appName}")]
    public class AppAssetsController: ToSic.Sxc.Oqt.Server.Controllers.AppAssetsController
    {
        public override string Route => "assets";

        public AppAssetsController(AppAssetsDependencies dependencies) : base(dependencies)
        { }
    }
}
