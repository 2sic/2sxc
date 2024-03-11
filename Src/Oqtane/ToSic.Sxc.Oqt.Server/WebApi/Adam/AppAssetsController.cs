using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Oqt.Server.Adam;

namespace ToSic.Sxc.Oqt.Server.WebApi.Adam;

// Release routes
[Route(OqtWebApiConstants.AppRootNoLanguage + "/{appName}/adam")]
[Route(OqtWebApiConstants.AppRootPathOrLang + "/{appName}/adam")]
[Route(OqtWebApiConstants.AppRootPathAndLang + "/{appName}/adam")]

// Beta routes
//[Route(WebApiConstants.WebApiStateRoot + "/adam/{appName}")]

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppAssetsController(AppAssetsControllerBase.MyServices services)
    : WebApi.AppAssetsControllerBase(services, OqtAssetsFileHelper.RouteAdam, "Assets");