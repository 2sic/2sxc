#if NETFRAMEWORK
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Context;
using ToSic.Eav.Internal.Licenses;
using ToSic.Sxc.Services;
using ToSic.Testing.Shared.Platforms;
using ToSic.Sxc.Configuration.Internal;
using ToSic.Testing.Shared;

namespace ToSic.Sxc.Tests.ServicesTests.ImageServiceTests;

/// <summary>
/// Run tests with Patrons not enabled.
/// </summary>
[TestClass]
public class ImageServiceTagsNoPatron() : ImageServiceTagsBase(EavTestConfig.ScenarioBasic)
{
    protected override void SetupServices(IServiceCollection services)
    {
        base.SetupServices(services);
        services.AddTransient<IPlatformInfo, TestPlatformNotPatron>();
    }

    [TestMethod]
    public void VerifyNotPatron()
    {
        var platform = GetService<IPlatformInfo>();
        Assert.AreEqual(platform.Name, new TestPlatformNotPatron().Name);
    }

    [TestMethod]
    public void VerifyFeatureSetDisabled()
    {
        var licenseSvc = GetService<ILicenseService>();
        var enabled = licenseSvc.IsEnabled(BuiltInLicenses.PatronPerfectionist);
        Assert.IsFalse(enabled);
    }

    [TestMethod]
    public void VerifyNotMultiSrcSet()
    {
        var features = GetService<IFeaturesService>();
        var enabled = features.IsEnabled(SxcFeatures.ImageServiceMultipleSizes.NameId);
        Assert.IsFalse(enabled);
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
#endif