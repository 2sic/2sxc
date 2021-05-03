using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Controllers.Adam
{
    // Release routes
    [Route(WebApiConstants.AppRoot + "/{appName}/adam")]
    [Route(WebApiConstants.AppRoot2 + "/{appName}/adam")]
    [Route(WebApiConstants.AppRoot3 + "/{appName}/adam")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/adam/{appName}")]
    public class AppAssetsController: Controllers.AppAssetsController
    {
        public override string Route => "adam";

        public AppAssetsController(AppAssetsDependencies dependencies) : base(dependencies)
        {

        }
    }
}
