using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.WebApi.Adam
{
    // Release routes
    [Route(WebApiConstants.AppRootNoLanguage + "/{appName}/adam")]
    [Route(WebApiConstants.AppRootPathOrLang + "/{appName}/adam")]
    [Route(WebApiConstants.AppRootPathNdLang + "/{appName}/adam")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/adam/{appName}")]

    public class AppAssetsController: WebApi.AppAssetsControllerBase
    {
        //protected override string HistoryLogName => "Oqt.AppAdm";
        
        public override string Route => "adam";

        public AppAssetsController(AppAssetsDependencies dependencies) : base(dependencies, "Assets")
        {

        }

    }
}
