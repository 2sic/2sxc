using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi.Admin.Metadata;
using ToSic.Sxc.WebApi;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    /// <inheritdoc cref="IMetadataController" />
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public class MetadataController : SxcApiControllerBase<MetadataControllerReal>, IMetadataController
    {
        public MetadataController() : base(MetadataControllerReal.LogSuffix) { }

        [HttpGet]
        public MetadataListDto Get(int appId, int targetType, string keyType, string key, string contentType = null)
            => Real.Get(appId, targetType, keyType, key, contentType);

    }
}