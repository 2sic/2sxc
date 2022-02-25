using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Oqt.Server.Adam;

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
        public AppAssetsController(AppAssetsDependencies dependencies) 
            : base(dependencies, ContentFileHelper.RouteAdam, "Assets") { }
    }
}
