using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.WebApi;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    /// <inheritdoc cref="IMetadataController" />
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public class MetadataController : SxcApiControllerBase, IMetadataController
    {
        [HttpGet]
        public IEnumerable<IDictionary<string, object>> Get(int appId, int targetType, string keyType, string key, string contentType)
            => GetService<MetadataBackend>().Get(appId, targetType, keyType, key, contentType);
    }
}