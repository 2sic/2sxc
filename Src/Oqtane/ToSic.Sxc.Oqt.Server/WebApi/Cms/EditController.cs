using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Sys.Cms;
using ToSic.Sxc.Oqt.Server.Controllers;
using RealController = ToSic.Sxc.Backend.Cms.EditControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Cms;

// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + $"/{AreaRoutes.Cms}")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Cms}")]
[Route(OqtWebApiConstants.ApiRootPathAndLang + $"/{AreaRoutes.Cms}")]

[ValidateAntiForgeryToken]

[ApiController]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class EditController() : OqtStatefulControllerBase(RealController.LogSuffix), IEditController
{
    private RealController Real => GetService<RealController>();


    [HttpPost]
    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    [AllowAnonymous]   // will check security internally, so assume no requirements
    public EditLoadDto Load([FromBody] List<ItemIdentifier> items, int appId)
        => Real.Load(items, appId);

    [HttpPost]
    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    [Authorize(Roles = RoleNames.Admin)]
    public Dictionary<Guid, int> Save([FromBody] EditSaveDto package, int appId, bool partOfPage)
        => Real.Save(package, appId, partOfPage);

    /// <inheritdoc />
    [HttpGet]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    //[Authorize(Roles = RoleNames.Everyone)] commented because of http403 issue
    // TODO: 2DM please check permissions
    public LinkInfoDto LinkInfo(string link, int appId, string contentType = default, Guid guid = default, string field = default)
        => Real.LinkInfo(link, appId, contentType, guid, field);

    /// <inheritdoc />
    [HttpPost]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    [Authorize(Roles = RoleNames.Admin)]
    public bool Publish(int id)
        => Real.Publish(id);
}