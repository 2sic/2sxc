using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
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
        public IEnumerable<Dictionary<string, object>> Get(int appId, int targetType, string keyType, string key, string contentType)
            => Eav.WebApi.MetadataApi.Get(ServiceProvider, appId, targetType, keyType, key, contentType);
    }
}