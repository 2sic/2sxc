using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    /// <inheritdoc cref="IMetadataController" />
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]

    // Release routes
    [Route(WebApiConstants.ApiRoot + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot2 + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot3 + "/admin/[controller]/[action]")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/admin/[controller]/[action]")]
    public class MetadataController : OqtStatefulControllerBase, IMetadataController
    {
        public MetadataBackend Backend { get; }
        protected override string HistoryLogName => "Api.Metadata";

        public MetadataController(MetadataBackend backend)
        {
            Backend = backend;
        }

        [HttpGet]
        public IEnumerable<IDictionary<string, object>> Get(int appId, int targetType, string keyType, string key, string contentType = null)
            => Backend.Get(appId, targetType, keyType, key, contentType);

    }
}