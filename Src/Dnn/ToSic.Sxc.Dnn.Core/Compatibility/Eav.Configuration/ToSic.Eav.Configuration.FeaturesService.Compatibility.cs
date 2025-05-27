﻿using ToSic.Sys.Capabilities.Features;

// ReSharper disable once CheckNamespace
namespace ToSic.Eav.Configuration;

/// <summary>
/// Implementation for an old API which was used in some Dnn Apps
/// Once it works, we will move it do Dnn only, so it won't work in Oqtane.
/// </summary>
internal class FeaturesServiceCompatibility: IFeaturesService
{
    private readonly ISysFeaturesService _featsInternal;

    public FeaturesServiceCompatibility(ISysFeaturesService featsInternal)
    {
        _featsInternal = featsInternal;
    }

    public bool Enabled(Guid guid) => _featsInternal.IsEnabled(guid);

    public bool Enabled(IEnumerable<Guid> guids) => _featsInternal.IsEnabled(guids);

    public bool Valid => _featsInternal.Valid;
}