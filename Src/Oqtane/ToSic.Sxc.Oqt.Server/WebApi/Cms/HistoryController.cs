using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Sys.Cms;
using ToSic.Sxc.Oqt.Server.Controllers;
using RealController = ToSic.Sxc.Backend.Cms.HistoryControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Cms;

/// <summary>
/// Controller for history of entities
/// </summary>
// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + $"/{AreaRoutes.Cms}")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Cms}")]
[Route(OqtWebApiConstants.ApiRootPathAndLang + $"/{AreaRoutes.Cms}")]
[PrivateApi]
[ValidateAntiForgeryToken]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class HistoryController() : OqtControllerBase(false, RealController.LogSuffix), IHistoryController
{
    /// <inheritdoc />
    public override ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}", null, GetType().Name);

    private RealController Real => GetService<RealController>();


    /// <inheritdoc />
    [HttpPost]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public bool Restore(int appId, [FromQuery(Name = "changeId")] int transactionId, [FromBody] ItemIdentifier item) 
        => Real.Restore(appId, transactionId, item);
}