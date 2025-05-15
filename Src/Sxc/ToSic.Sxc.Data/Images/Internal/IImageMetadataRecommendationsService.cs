using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Images.Internal;

public interface IImageMetadataRecommendationsService
{
    /// <summary>
    /// Optionally add image-metadata recommendations
    /// </summary>
    void SetImageRecommendations(IMetadataOf mdOf, string path);

    string[] GetImageRecommendations();
}