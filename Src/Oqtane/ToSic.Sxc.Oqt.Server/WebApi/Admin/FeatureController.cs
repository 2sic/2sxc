﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Sys.Admin.Features;
using ToSic.Eav.WebApi.Sys.Licenses;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sys.Capabilities.Features;
using RealController = ToSic.Eav.WebApi.Sys.Admin.Features.FeatureControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin;

[ValidateAntiForgeryToken]

// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathAndLang + $"/{AreaRoutes.Admin}")]

[ShowApiWhenReleased(ShowApiMode.Never)]
public class FeatureController() : OqtStatefulControllerBase(RealController.LogSuffix), IFeatureController
{
    private RealController Real => GetService<RealController>();

    [HttpGet]
    [Authorize(Roles = RoleNames.Admin)]
    public FeatureStateDto Details(string nameId) => Real.Details(nameId);

    /// <summary>
    /// POST updated features JSON configuration.
    /// </summary>
    /// <remarks>
    /// Added in 2sxc 13
    /// </remarks>
    [HttpPost]
    [Authorize(Roles = RoleNames.Host)]
    public bool SaveNew([FromBody] List<FeatureStateChange> changes) => Real.SaveNew(changes);

}