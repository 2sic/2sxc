using ToSic.Eav.WebApi.Sys.Admin.Metadata;
using RealController = ToSic.Eav.WebApi.Sys.Admin.Metadata.MetadataControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

/// <inheritdoc cref="IMetadataController" />
[SupportedModules(DnnSupportedModuleNames)]
[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
[ValidateAntiForgeryToken]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class MetadataController() : DnnSxcControllerBase(RealController.LogSuffix), IMetadataController
{
    private RealController Real => SysHlp.GetService<RealController>();

    [HttpGet]
    public MetadataListDto Get(int appId, int targetType, string keyType, string key, string contentType = null)
        => Real.Get(appId, targetType, keyType, key, contentType);

}