using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Images.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IImageMetadataRecommendationsService
{
    /// <summary>
    /// Optionally add image-metadata recommendations
    /// </summary>
    void SetImageRecommendations(IMetadata? mdOf, string? path);

    string[] GetImageRecommendations();
}