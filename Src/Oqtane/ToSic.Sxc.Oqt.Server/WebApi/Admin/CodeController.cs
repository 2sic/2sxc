using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.WebApi;
using RealController = ToSic.Sxc.Backend.Admin.CodeControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin;

// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathAndLang + $"/{AreaRoutes.Admin}")]

[ValidateAntiForgeryToken]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CodeController() : OqtControllerBase(false, RealController.LogSuffix)
{
    private RealController Real => GetService<RealController>();

    [HttpGet]
    [Authorize(Roles = RoleNames.Admin)]
    public IEnumerable<RealController.HelpItem> InlineHelp(string language) => Real.InlineHelp(language);

    [HttpGet]
    [Authorize(Roles = RoleNames.Host)]
    public RichResult GenerateDataModels(int appId, string generator, string edition = default) => Real.GenerateDataModels(appId, edition, generator: generator);

    [HttpGet]
    [JsonFormatter]
    [Authorize(Roles = RoleNames.Host)]
    public EditionsDto GetEditions(int appId) => Real.GetEditions(appId);
}