using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.WebApi.Sys.ActionFilters;
using RealController = ToSic.Sxc.Backend.Admin.CodeControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin;

// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathAndLang + $"/{AreaRoutes.Admin}")]

[ValidateAntiForgeryToken]
[OqtLogWebApi]
[OqtInitializeAlias]
[NoContentForEmptyAction]
[SystemTextJsonFormatter]
[ServiceFilter(typeof(OptionalBodyFilter))]
[ServiceFilter(typeof(HttpResponseExceptionFilter))]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class CodeController : ControllerBase, IHasLog
{
    /// <inheritdoc />
    public ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}", null, GetType().Name);

    private RealController Real => this.GetApiService<RealController>();

    [HttpGet]
    [Authorize(Roles = RoleNames.Admin)]
    public IEnumerable<RealController.HelpItem> InlineHelp(string language)
        => Real.InlineHelp(language);

    [HttpGet]
    [Authorize(Roles = RoleNames.Host)]
    public RichResult GenerateDataModels(int appId, string generator, string edition = null, int configurationId = 0)
        => Real.GenerateDataModels(appId, edition, generator: generator, configurationId: configurationId);
}
