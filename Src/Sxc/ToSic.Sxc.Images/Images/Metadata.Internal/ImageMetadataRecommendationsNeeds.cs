using ToSic.Sxc.Data;

namespace ToSic.Sxc.Images.Metadata.Internal;

/// <summary>
/// Interface to explain what the Image Metadata Recommendations need.
/// Must be implemented by the CodeApiService / CodeContext
/// </summary>
public record ImageMetadataRecommendationsNeeds(ITypedStack AllSettings);