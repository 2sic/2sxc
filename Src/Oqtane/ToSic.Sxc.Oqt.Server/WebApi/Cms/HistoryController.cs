using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.Ui.Edit.Sys;
using ToSic.Eav.Ui.History.Sys;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.WebApi.Sys.ActionFilters;
using RealController = ToSic.Eav.Ui.History.Sys.HistoryControllerReal;

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
[OqtLogWebApi, OqtInitializeAlias, NoContentForEmptyAction]
[SystemTextJsonFormatter, ServiceFilter(typeof(OptionalBodyFilter)), ServiceFilter(typeof(HttpResponseExceptionFilter))]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class HistoryController : ControllerBase, IHasLog, IHistoryController
{
    /// <inheritdoc />
    public ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}", null, GetType().Name);

    private RealController Real => this.GetApiService<RealController>();


    /// <inheritdoc />
    [HttpPost]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public bool Restore(int appId, [FromQuery(Name = "changeId")] int transactionId, [FromBody] ItemIdentifier item) 
        => Real.Restore(appId, transactionId, item);
}