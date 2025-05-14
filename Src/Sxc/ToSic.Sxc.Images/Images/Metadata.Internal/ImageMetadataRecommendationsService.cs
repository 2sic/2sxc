using System.IO;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.Metadata;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Sys.ExecutionContext;
using IFeaturesService = ToSic.Sxc.Services.IFeaturesService;

namespace ToSic.Sxc.Images.Internal;

/// <summary>
/// Small service to image metadata additional recommendations as configured.
/// </summary>
class ImageMetadataRecommendationsService(IFeaturesService featuresSvc) : ServiceForDynamicCode("Img.MdRecS", connect: [featuresSvc]), IImageMetadataRecommendationsService
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

    // TODO: THIS IS ALL very temporary - it should be in a proper service, 
    // but because we'll need to merge the code with v17, we try do keep it this way.
    public string[] GetImageRecommendations()
    {
        if (_CodeApiSvc is not IExConAllSettings codeRootTyped)
            return ImageRecommendationsBasic;

        if (!featuresSvc.IsEnabled(BuiltInFeatures.CopyrightManagement.NameId))
            return ImageRecommendationsBasic;

        var useCopyright = codeRootTyped.AllSettings?.Bool($"Copyright.{nameof(CopyrightSettings.ImagesInputEnabled)}") ?? false;
        return useCopyright
            ? ImageRecommendationsCopyright
            : ImageRecommendationsBasic;
    }
    private static string[] ImageRecommendationsBasic => [ImageDecorator.TypeNameId];
    private static string[] ImageRecommendationsCopyright => [CopyrightDecorator.TypeNameId, ImageDecorator.TypeNameId];
}