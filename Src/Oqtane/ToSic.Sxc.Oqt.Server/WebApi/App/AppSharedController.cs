using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Oqt.Server.Adam;

namespace ToSic.Sxc.Oqt.Server.WebApi.App
{
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)] // use view, all methods must re-check permissions
    //[Authorize(Roles = RoleNames.Everyone)] commented because of http403 issue
    // TODO: 2DM please check permissions

    // Release routes
    [Route(OqtWebApiConstants.SharedRootNoLanguage + "/{appName}/")]
    [Route(OqtWebApiConstants.SharedRootPathOrLang + "/{appName}/")]
    [Route(OqtWebApiConstants.SharedRootPathNdLang + "/{appName}/")]
    public class AppSharedController: AppAssetsControllerBase
    {
        public AppSharedController(MyServices services) 
            : base(services, OqtAssetsFileHelper.RouteShared, "Shared") { }
    }
}
