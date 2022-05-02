using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Oqt.Server.Adam;

namespace ToSic.Sxc.Oqt.Server.WebApi.App
{
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)] // use view, all methods must re-check permissions
    //[Authorize(Roles = RoleNames.Everyone)] commented because of http403 issue
    // TODO: 2DM please check permissions

    // Release routes
    [Route(WebApiConstants.SharedRootNoLanguage + "/{appName}/")]
    [Route(WebApiConstants.SharedRootPathOrLang + "/{appName}/")]
    [Route(WebApiConstants.SharedRootPathNdLang + "/{appName}/")]
    public class AppSharedController: AppAssetsControllerBase
    {
        public AppSharedController(AppAssetsDependencies dependencies) 
            : base(dependencies, ContentFileHelper.RouteShared, "Shared") { }
    }
}
