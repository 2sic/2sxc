using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Context;
using ToSic.Eav.Internal.Licenses;
using ToSic.Eav.Testing;
using ToSic.Sxc.Services;
using ToSic.Testing.Shared.Platforms;
using ToSic.Sxc.Configuration.Internal;

namespace ToSic.Sxc.Tests.ServicesTests.ImageServiceTests;

/// <summary>
/// Run tests with Patrons not enabled.
/// </summary>
[TestClass]
public class ImageServiceTagsNoPatron() : ImageServiceTagsBase(new ScenarioBasic())
{
    protected override IServiceCollection SetupServices(IServiceCollection services) =>
        base.SetupServices(services)
            .AddTransient<IPlatformInfo, TestPlatformNotPatron>();

    [TestMethod]
    public void VerifyNotPatron()
    {
        var platform = GetService<IPlatformInfo>();
        AreEqual(platform.Name, new TestPlatformNotPatron().Name);
    }

    [TestMethod]
    public void VerifyFeatureSetDisabled()
    {
        var licenseSvc = GetService<ILicenseService>();
        var enabled = licenseSvc.IsEnabled(BuiltInLicenses.PatronPerfectionist);
        IsFalse(enabled);
    }

    [TestMethod]
    public void VerifyNotMultiSrcSet()
    {
        var features = GetService<IFeaturesService>();
        var enabled = features.IsEnabled(SxcFeatures.ImageServiceMultipleSizes.NameId);
        IsFalse(enabled);
    }

    protected override bool TestModeImg => false;


    [DataRow(SrcJpgNone, SrcSetNone, "No Src Set, no patron")]
    [DataRow(SrcJpgNone, SrcSet12, "With Src Set 1,2; no patron")]
    [DataTestMethod]
    public void SourceTagsMultiTests(string expected, string variants, string name) 
        => BatchTestManySrcSets(expected, variants, name);


    [DataRow(SrcJpgNone, SrcSetNone, true, "No Src Set, in-pic")]
    [DataRow(SrcJpgNone, SrcSetNone, false, "No Src Set, in-settings")]
    [DataRow(SrcJpgNone, SrcSet12, true, "With Src Set 1,2, in-pic")]
    [DataRow(SrcJpgNone, SrcSet12, false, "With Src Set 1,2, in-settings")]
    [DataTestMethod]
    public void PictureTags(string expected, string variants, bool inPicTag, string name) 
        => PictureTagInner(expected, variants, inPicTag, name);
}