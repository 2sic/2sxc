using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    /// <inheritdoc cref="IMetadataController" />
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]

    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Admin}")]

    // Beta routes - TODO: @STV - why is this beta?
    [Route(WebApiConstants.WebApiStateRoot + $"/{AreaRoutes.Admin}")]
    public class MetadataController : OqtStatefulControllerBase<DummyControllerReal>, IMetadataController
    {
        public MetadataController(MetadataBackend backend): base("MetaDt")
        {
            Backend = backend;
        }
        public MetadataBackend Backend { get; }

        [HttpGet]
        public MetadataListDto Get(int appId, int targetType, string keyType, string key, string contentType = null)
            => Backend.Init(Log).Get(appId, targetType, keyType, key, contentType);

    }
}