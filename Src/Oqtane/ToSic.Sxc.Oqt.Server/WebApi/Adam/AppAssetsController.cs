using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Oqt.Server.Adam;

namespace ToSic.Sxc.Oqt.Server.WebApi.Adam
{
    // Release routes
    [Route(OqtWebApiConstants.AppRootNoLanguage + "/{appName}/adam")]
    [Route(OqtWebApiConstants.AppRootPathOrLang + "/{appName}/adam")]
    [Route(OqtWebApiConstants.AppRootPathNdLang + "/{appName}/adam")]

    // Beta routes
    //[Route(WebApiConstants.WebApiStateRoot + "/adam/{appName}")]

    public class AppAssetsController: WebApi.AppAssetsControllerBase
    {
        public AppAssetsController(MyServices services) 
            : base(services, OqtAssetsFileHelper.RouteAdam, "Assets") { }
    }
}
