using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Admin.Metadata;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
using RealController = ToSic.Eav.WebApi.Admin.Metadata.MetadataControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin;

/// <inheritdoc cref="IMetadataController" />
[ValidateAntiForgeryToken]
[Authorize(Roles = RoleNames.Admin)]

// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathAndLang + $"/{AreaRoutes.Admin}")]

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class MetadataController() : OqtStatefulControllerBase(RealController.LogSuffix), IMetadataController
{
    private RealController Real => GetService<RealController>();


    [HttpGet]
    public MetadataListDto Get(int appId, int targetType, string keyType, string key, string contentType = null)
        => Real.Get(appId, targetType, keyType, key, contentType);

}