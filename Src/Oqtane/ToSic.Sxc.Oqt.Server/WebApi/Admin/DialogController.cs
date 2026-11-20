using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Admin.Sys;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.WebApi.Sys.ActionFilters;
using RealController = ToSic.Sxc.Backend.Admin.DialogControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin;

/// <summary>
/// This one supplies portal-wide (or cross-portal) settings / configuration
/// </summary>
//[SupportedModules("2sxc,2sxc-app")]
//[DnnLogExceptions]
[Authorize(Roles = RoleNames.Admin)]
[AutoValidateAntiforgeryToken]

// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathAndLang + $"/{AreaRoutes.Admin}")]

[ApiController]

[OqtLogWebApi]
[OqtInitializeAlias]
[NoContentForEmptyAction]
[SystemTextJsonFormatter]
[ServiceFilter(typeof(OptionalBodyFilter))]
[ServiceFilter(typeof(HttpResponseExceptionFilter))]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class DialogController : ControllerBase, IHasLog, IDialogController
{
    /// <inheritdoc />
    public ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}", null, GetType().Name);

    private RealController Real => this.GetApiService<RealController>();


    [HttpGet]
    public DialogContextStandaloneDto Settings(int appId)
    {
        this.PrepareRequestBlock();
        return Real.Settings(appId);
    }
        
}