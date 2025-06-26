using ToSic.Sys.Capabilities.Features;

// ReSharper disable once CheckNamespace
namespace ToSic.Eav.Configuration;

/// <summary>
/// Implementation for an old API which was used in some Dnn Apps
/// Once it works, we will move it do Dnn only, so it won't work in Oqtane.
/// </summary>
internal class FeaturesServiceCompatibility(ISysFeaturesService featsInternal) : IFeaturesService
{
    public bool Enabled(Guid guid) => featsInternal.IsEnabled(guid);

    public bool Enabled(IEnumerable<Guid> guids) => featsInternal.IsEnabled(guids);

    public bool Valid => featsInternal.Valid;
}