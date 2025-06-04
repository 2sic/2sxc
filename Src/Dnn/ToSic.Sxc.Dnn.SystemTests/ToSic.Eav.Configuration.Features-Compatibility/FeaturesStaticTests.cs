using ToSic.Eav.Configuration;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.Testing.Scenarios;
using ToSic.Sxc.Dnn.StartUp;
using ToSic.Sys.Capabilities.Features;
using BuiltInFeatures = ToSic.Sys.Capabilities.Features.BuiltInFeatures;

#pragma warning disable 618

namespace ToSic.Sxc.ToSic.Eav.Configuration.Features_Compatibility;

// ReSharper disable once InconsistentNaming
public class FeaturesStaticTests : IClassFixture<DoFixtureStartup<ScenarioFullPatronsWithDb>>
{
    public FeaturesStaticTests(ISysFeaturesService featuresSvc)
    {
        new StartupDnn().SetupOldStaticFeaturesForCompatibility(featuresSvc);
    }

    [Fact]
    //[Ignore("I believe the setup doesn't work yet - needs to first load the licenses to be able to test this")]
    public void PasteClipboardActive()
    {
        var x = Features.Enabled(BuiltInFeatures.PasteImageFromClipboard.Guid);
        True(x, "this should be enabled and non-expired");
    }

    [Fact]
    //[Ignore("I believe the setup doesn't work yet - needs to first load the licenses to be able to test this")]
    public void InventedFeatureGuid()
    {
        var inventedGuid = new Guid("12345678-1c8b-4286-a33b-3210ed3b2d9a");
        var x = Features.Enabled(inventedGuid);
        False(x, "this should be enabled and expired");
    }
}