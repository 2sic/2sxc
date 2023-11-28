using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System.Collections.Generic;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.WebApi.Admin.Features;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
using RealController = ToSic.Eav.WebApi.Admin.Features.FeatureControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin;

[ValidateAntiForgeryToken]

// Release routes
[Route(OqtWebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Admin}")]

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class FeatureController : OqtStatefulControllerBase, IFeatureController
{
    public FeatureController(): base(RealController.LogSuffix) { }

    private RealController Real => GetService<RealController>();


    /// <summary>
    /// POST updated features JSON configuration.
    /// </summary>
    /// <remarks>
    /// Added in 2sxc 13
    /// </remarks>
    [HttpPost]
    [Authorize(Roles = RoleNames.Host)]
    public bool SaveNew([FromBody] List<FeatureManagementChange> changes) => Real.SaveNew(changes);

}