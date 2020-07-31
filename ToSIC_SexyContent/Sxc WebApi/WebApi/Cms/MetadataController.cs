using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi.PublicApi;

namespace ToSic.Sxc.WebApi.Cms
{
    /// <inheritdoc cref="IMetadataController" />
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public class MetadataController : SxcApiControllerBase, IMetadataController
    {
        [HttpGet]
        public IEnumerable<Dictionary<string, object>> Get(int appId, int targetType, string keyType, string key, string contentType)
            => Eav.WebApi.MetadataController.Get(appId, targetType, keyType, key, contentType);
    }
}