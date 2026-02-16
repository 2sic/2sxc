using ToSic.Eav.Metadata;
using ToSic.Sxc.Adam.Sys;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Capabilities.Features;
using ToSic.Sys.Utils;
using IFeaturesService = ToSic.Sxc.Services.IFeaturesService;

namespace ToSic.Sxc.Images.Sys.Metadata;

/// <summary>
/// Small service to image metadata additional recommendations as configured.
/// </summary>
class ImageMetadataRecommendationsService(IFeaturesService featuresSvc) : ServiceWithContext("Img.MdRecS", connect: [featuresSvc]), IImageMetadataRecommendationsService
{
    /// <summary>
    /// Optionally add image-metadata recommendations
    /// </summary>
    public void SetImageRecommendations(IMetadata? mdOf, string? path)
    {
        if (mdOf?.Target == null || !path.HasValue())
            return;
        var ext = Path.GetExtension(path);
        if (ext.HasValue() && AssetTypeNames.IsImage(ext))
            mdOf.Target.Recommendations = GetImageRecommendations();
    }

    public string[] GetImageRecommendations()
    {
        var sysSettings = ExCtxOrNull?.GetDataStack<ITypedStack>(ExecutionContextStateNames.AllSettings);
        if (sysSettings == null || !featuresSvc.IsEnabled(BuiltInFeatures.CopyrightManagement.NameId))
            return ImageRecommendationsBasic;

        var useCopyright = sysSettings.Bool($"{CopyrightSettings.SettingsPath}.{nameof(CopyrightSettings.ImagesInputEnabled)}");
        return useCopyright
            ? ImageRecommendationsCopyright
            : ImageRecommendationsBasic;
    }

    /// <summary>
    /// Basic recommendations for image metadata - just the ImageDecorator
    /// </summary>
    private static string[] ImageRecommendationsBasic => [ImageDecorator.TypeNameId];

    /// <summary>
    /// Advanced recommendations for image metadata (if enabled) - with additional Copyright
    /// </summary>
    private static string[] ImageRecommendationsCopyright => [CopyrightDecorator.ContentTypeNameId, ImageDecorator.TypeNameId];
}