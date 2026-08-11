using ToSic.Eav.WebApi.Sys.Admin.Metadata;
using ToSic.Sxc.Dnn.WebApi.Sys;
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

    // Implemented by DataSource System.ItemMetadata through query System.SysData.
    // probably 3 streams
    // - Recommendations
    // - Items (containing the entities)
    // - For (what the metadata is for)


}