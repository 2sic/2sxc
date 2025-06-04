using System.IO;
using ToSic.Eav.Metadata;

using ToSic.Sxc.Adam;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Capabilities.Features;
using ToSic.Sys.Utils;
using IFeaturesService = ToSic.Sxc.Services.IFeaturesService;

namespace ToSic.Sxc.Images.Internal;

/// <summary>
/// Small service to image metadata additional recommendations as configured.
/// </summary>
class ImageMetadataRecommendationsService(IFeaturesService featuresSvc) : ServiceWithContext("Img.MdRecS", connect: [featuresSvc]), IImageMetadataRecommendationsService
{
    /// <summary>
    /// Optionally add image-metadata recommendations
    /// </summary>
    public void SetImageRecommendations(IMetadataOf mdOf, string path)
    {
        if (mdOf?.Target == null || !path.HasValue())
            return;
        var ext = Path.GetExtension(path);
        if (ext.HasValue() && Classification.IsImage(ext))
            mdOf.Target.Recommendations = GetImageRecommendations();
    }

    public string[] GetImageRecommendations()
    {
        var settings = ExCtxOrNull?.GetState<ITypedStack>(ExecutionContextStateNames.AllSettings);
        if (settings == null || !featuresSvc.IsEnabled(BuiltInFeatures.CopyrightManagement.NameId))
            return ImageRecommendationsBasic;

        var useCopyright = settings.Bool($"Copyright.{nameof(CopyrightSettings.ImagesInputEnabled)}");
        return useCopyright
            ? ImageRecommendationsCopyright
            : ImageRecommendationsBasic;
    }
    private static string[] ImageRecommendationsBasic => [ImageDecorator.TypeNameId];
    private static string[] ImageRecommendationsCopyright => [CopyrightDecorator.TypeNameId, ImageDecorator.TypeNameId];
}