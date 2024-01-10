using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi.Admin.Metadata;
using ToSic.Sxc.Dnn.WebApi.Internal;
using ToSic.Sxc.WebApi;
using RealController = ToSic.Eav.WebApi.Admin.Metadata.MetadataControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

/// <inheritdoc cref="IMetadataController" />
[SupportedModules(DnnSupportedModuleNames)]
[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
[ValidateAntiForgeryToken]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class MetadataController() : DnnSxcControllerBase(RealController.LogSuffix), IMetadataController
{
    private RealController Real => SysHlp.GetService<RealController>();

    [HttpGet]
    public MetadataListDto Get(int appId, int targetType, string keyType, string key, string contentType = null)
        => Real.Get(appId, targetType, keyType, key, contentType);

}