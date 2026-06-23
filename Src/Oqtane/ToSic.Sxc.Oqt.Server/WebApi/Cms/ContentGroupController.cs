using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Sxc.Backend.Cms;
using ToSic.Sxc.Oqt.Server.Controllers;
using RealController = ToSic.Sxc.Backend.Cms.ContentGroupControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Cms;

// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + $"/{AreaRoutes.Cms}")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Cms}")]
[Route(OqtWebApiConstants.ApiRootPathAndLang + $"/{AreaRoutes.Cms}")]

[ValidateAntiForgeryToken]
[ApiController]
// cannot use this, as most requests now come from a lone page [SupportedModules("2sxc,2sxc-app")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ContentGroupController() : OqtStatefulControllerBase(RealController.LogSuffix), IContentGroupController
{
    private RealController Real => GetService<RealController>();


    [HttpGet]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public EntityInListDto Header(Guid guid)
        => Real.Header(guid);


    [HttpGet]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public List<EntityInListDto> ItemList(Guid parent, string part)
        => Real.ItemList(parent, part);


    // TODO: part should be handed in with all the relevant names! atm it's "content" in the content-block scenario
    [HttpPost]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public bool ItemList([FromQuery] Guid parent, List<EntityInListDto> list, [FromQuery] string part = null)
        => Real.ItemList(parent, list, part);
}