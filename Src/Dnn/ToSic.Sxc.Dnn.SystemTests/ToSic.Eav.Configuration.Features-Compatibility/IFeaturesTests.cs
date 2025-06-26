using ToSic.Eav.Configuration;
using ToSic.Eav.Testing.Scenarios;
using ToSic.Sys.Capabilities.Features;

namespace ToSic.Sxc.ToSic.Eav.Configuration.Features_Compatibility;

// ReSharper disable once InconsistentNaming
public class IFeaturesTests(IFeaturesService Features) : IClassFixture<DoFixtureStartup<ScenarioFullPatronsWithDb>>
{
    [Fact]
    public void PasteClipboardActive()
    {
        var x = Features.Enabled(BuiltInFeatures.PasteImageFromClipboard.Guid);
        True(x, "this should be enabled and non-expired");
    }

    [Fact]
    public void InventedFeatureGuid()
    {
        var inventedGuid = new Guid("12345678-1c8b-4286-a33b-3210ed3b2d9a");
        var x = Features.Enabled(inventedGuid);
        False(x, "this should be enabled and expired");
    }
}