using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Oqt.Server.Adam;

namespace ToSic.Sxc.Oqt.Server.WebApi.App;
//[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)] // use view, all methods must re-check permissions
//[Authorize(Roles = RoleNames.Everyone)] commented because of http403 issue
// TODO: 2DM please check permissions

// Release routes
[Route(OqtWebApiConstants.AppRootNoLanguage + "/{appName}/assets")]
[Route(OqtWebApiConstants.AppRootPathOrLang + "/{appName}/assets")]
[Route(OqtWebApiConstants.AppRootPathAndLang + "/{appName}/assets")]

// Beta routes
//[Route(WebApiConstants.WebApiStateRoot + "/assets/{appName}")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppAssetsController(AppAssetsControllerBase.MyServices services)
    : AppAssetsControllerBase(services, OqtAssetsFileHelper.RouteAssets, "Assets");