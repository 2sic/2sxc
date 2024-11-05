using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System;
using ToSic.Eav.WebApi.Routing;
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
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ContentGroupController() : OqtStatefulControllerBase(RealController.LogSuffix), IContentGroupController
{
    private RealController Real => GetService<RealController>();


    [HttpGet]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public EntityInListDto Header(Guid guid)
        => Real.Header(guid);


    // TODO: shouldn't be part of ContentGroupController any more, as it's generic now
    [HttpPost]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public void Replace(Guid guid, string part, int index, int entityId, bool add = false)
        => Real.Replace(guid, part, index, entityId, add);


    // TODO: WIP changing this from ContentGroup editing to any list editing
    [HttpGet]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public ReplacementListDto Replace(Guid guid, string part, int index)
        => Real.Replace(guid, part, index);

    [HttpGet]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public List<EntityInListDto> ItemList(Guid guid, string part)
        => Real.ItemList(guid, part);


    // TODO: part should be handed in with all the relevant names! atm it's "content" in the content-block scenario
    [HttpPost]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public bool ItemList([FromQuery] Guid guid, List<EntityInListDto> list, [FromQuery] string part = null)
        => Real.ItemList(guid, list, part);
}