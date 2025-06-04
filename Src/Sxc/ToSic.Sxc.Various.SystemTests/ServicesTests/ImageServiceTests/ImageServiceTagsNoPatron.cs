using ToSic.Sxc.Configuration.Internal;
using ToSic.Sxc.Services;
using ToSic.Sys.Capabilities.Licenses;
using ToSic.Sys.Capabilities.Platform;
using ToSic.Testing.Shared.Platforms;
using Xunit.Abstractions;

namespace ToSic.Sxc.ServicesTests.ImageServiceTests;

// TODO: SOME OF THESE tests should probably go to the configuration folder...

/// <summary>
/// Run tests with Patrons not enabled.
/// </summary>
[Startup(typeof(StartupSxcWithDbBasic))]
public class ImageServiceTagsNoPatron(IImageService svc, ITestOutputHelper output, IPlatformInfo platform, ILicenseService licenseSvc, IFeaturesService features)
    : ImageServiceTagsBase(svc, output, new ScenarioBasic()), IClassFixture<DoFixtureStartup<ScenarioBasic>>
{
    [Fact]
    public void VerifyNotPatron() => Equal(platform.Name, new TestPlatformNotPatron().Name);

    [Fact]
    public void VerifyFeatureSetDisabled()
    {
        var enabled = licenseSvc.IsEnabled(BuiltInLicenses.PatronPerfectionist);
        False(enabled);
    }

    [Fact]
    public void VerifyNotMultiSrcSet()
    {
        var enabled = features.IsEnabled(SxcFeatures.ImageServiceMultipleSizes.NameId);
        False(enabled);
    }

    protected override bool TestModeImg => false;


    [Theory]
    [InlineData(SrcJpgNone, SrcSetNone, "No Src Set, no patron")]
    [InlineData(SrcJpgNone, SrcSet12, "With Src Set 1,2; no patron")]
    public void SourceTagsMultiTests(string expected, string variants, string name) 
        => BatchTestManySrcSets(expected, variants, name);


    [Theory]
    [InlineData(SrcJpgNone, SrcSetNone, true, "No Src Set, in-pic")]
    [InlineData(SrcJpgNone, SrcSetNone, false, "No Src Set, in-settings")]
    [InlineData(SrcJpgNone, SrcSet12, true, "With Src Set 1,2, in-pic")]
    [InlineData(SrcJpgNone, SrcSet12, false, "With Src Set 1,2, in-settings")]
    public void PictureTags(string expected, string variants, bool inPicTag, string name) 
        => PictureTagInner(expected, variants, inPicTag, name);
}