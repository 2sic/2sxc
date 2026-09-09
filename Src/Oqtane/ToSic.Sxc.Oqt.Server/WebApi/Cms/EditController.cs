using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Sys.Cms;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sys.Logging;
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
    private static readonly ActivitySource Activities = new("ToSic.2sxc.WebApi");
    private RealController Real => GetService<RealController>();


    [HttpPost]
    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    [AllowAnonymous]   // will check security internally, so assume no requirements
    public async Task<EditLoadDto> Load([FromBody] List<ItemIdentifier> items, int appId)
    {
        var services = HttpContext.RequestServices;
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(MicrosoftLoggerEventSink.Category);
        using var execution = logger.BeginExecution(Log, Activities, "Edit.Load", appId: appId);
        logger.LogTrace("Loading edit data for app {AppId} with {ItemCount} items", appId, items?.Count);
        // Legacy/Compare still need parent propagation into their original entry buffers.
        var backend = services.GetRequiredService<ILogStoreLive>().Mode == LogStoreMode.ILogger
            ? services.Build<RealController>()
            : Real;
        var result = await backend.Load(items, appId);
        logger.LogTrace("Loaded edit data for app {AppId}", appId);
        return result;
    }

    [HttpPost]
    // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<Dictionary<Guid, int>> Save([FromBody] EditSaveDto package, int appId, bool partOfPage)
        => await Real.Save(package, appId, partOfPage);

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
