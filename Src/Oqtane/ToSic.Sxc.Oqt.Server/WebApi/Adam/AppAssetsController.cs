using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.WebApi.Adam
{
    // Release routes
    [Route(WebApiConstants.AppRoot + "/{appName}/adam")]
    [Route(WebApiConstants.AppRoot2 + "/{appName}/adam")]
    [Route(WebApiConstants.AppRoot3 + "/{appName}/adam")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/adam/{appName}")]
    public class AppAssetsController: WebApi.AppAssetsControllerBase
    {
        protected override string HistoryLogName => "Oqt.AppAdm";
        
        public override string Route => "adam";

        public AppAssetsController(AppAssetsDependencies dependencies) : base(dependencies)
        {

        }

    }
}
