using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Dto;
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
        public MetadataController() : base("MetaDt") { }

        [HttpGet]
        public MetadataListDto Get(int appId, int targetType, string keyType, string key, string contentType = null)
            => GetService<MetadataBackend>().Init(Log).Get(appId, targetType, keyType, key, contentType);

    }
}