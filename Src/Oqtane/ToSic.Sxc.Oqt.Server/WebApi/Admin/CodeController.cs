using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System.Collections.Generic;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.WebApi.Admin;
using RealController = ToSic.Sxc.WebApi.Admin.CodeControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin;

// Release routes
[Route(OqtWebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Admin}")]

[ValidateAntiForgeryToken]
//[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
[Authorize(Roles = RoleNames.Admin)]

public class CodeController : OqtControllerBase
{
    public CodeController() : base(false, RealController.LogSuffix) { }

    private RealController Real => GetService<RealController>();


    [HttpGet]
    public IEnumerable<CodeControllerReal.HelpItem> InlineHelp(string language) => Real.InlineHelp(language);
}