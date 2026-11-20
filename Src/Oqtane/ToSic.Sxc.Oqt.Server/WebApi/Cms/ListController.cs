using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Sys.Cms;
using ToSic.Sxc.Backend.Cms;
using ToSic.Sxc.Oqt.Server.Controllers;
using RealController = ToSic.Sxc.Backend.Cms.ListControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Cms;

// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + $"/{AreaRoutes.Cms}")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Cms}")]
[Route(OqtWebApiConstants.ApiRootPathAndLang + $"/{AreaRoutes.Cms}")]

[ValidateAntiForgeryToken]
//[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
[Authorize(Roles = RoleNames.Admin)]
[ShowApiWhenReleased(ShowApiMode.Never)]
[OqtPrepareBlockContext]
public class ListController() : OqtControllerBase(false, RealController.LogSuffix), IListController
{
    /// <inheritdoc />
    public override ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}", null, GetType().Name);

    private RealController Real => GetService<RealController>();

    [HttpPost]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public void Replace(Guid parent, string part, int index, int entityId, bool add = false)
        => Real.Replace(parent, part, index, entityId, add);


    [HttpGet]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public ListSlotSwapInfo ReplaceOptions(Guid parent, string part, int index, string? contentType = null)
        => Real.ReplaceOptions(parent, part, index, contentType);

    /// <inheritdoc />
    /// <summary>
    /// used to be GET Module/ChangeOrder
    /// </summary>
    [HttpPost]
    public void Move(Guid? parent, string fields, int index, int toIndex) 
        => Real.Move(parent, fields, index, toIndex);

    /// <inheritdoc />
    /// <summary>
    /// Used to be Get Module/RemoveFromList
    /// </summary>
    [HttpDelete]
    public void Delete(Guid? parent, string fields, int index) 
        => Real.Delete(parent, fields, index);

    [HttpGet]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public List<ListSlotContentsInfo> Items(Guid parent, string part)
        => Real.Items(parent, part);


    // TODO: part should be handed in with all the relevant names! atm it's "content" in the content-block scenario
    [HttpPost]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public bool Items([FromQuery] Guid parent, List<ListSlotContentsInfo> list, [FromQuery] string part)
        => Real.Items(parent, list, part);

    [HttpGet]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public List<ListSlotContentsInfo> ContentBlockHeader(Guid parent)
        => Real.ContentBlockHeader(parent);

}