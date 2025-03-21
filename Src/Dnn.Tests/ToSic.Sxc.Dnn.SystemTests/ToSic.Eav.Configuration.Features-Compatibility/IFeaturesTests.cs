﻿using ToSic.Eav.Configuration;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.Testing.Scenarios;

namespace ToSic.Sxc.ToSic.Eav.Configuration.Features_Compatibility;

// ReSharper disable once InconsistentNaming
public class IFeaturesTests(IFeaturesService Features) : IClassFixture<DoFixtureStartup<ScenarioFullPatrons>>
{
    [Fact]
    public void PasteClipboardActive()
    {
        var x = Features.Enabled(BuiltInFeatures.PasteImageFromClipboard.Guid);
        Assert.True(x, "this should be enabled and non-expired");
    }

    [Fact]
    public void InventedFeatureGuid()
    {
        var inventedGuid = new Guid("12345678-1c8b-4286-a33b-3210ed3b2d9a");
        var x = Features.Enabled(inventedGuid);
        Assert.False(x, "this should be enabled and expired");
    }
}