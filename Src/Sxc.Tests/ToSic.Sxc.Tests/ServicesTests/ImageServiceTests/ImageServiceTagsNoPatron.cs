using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Context;
using ToSic.Testing.Shared.Platforms;

namespace ToSic.Sxc.Tests.ServicesTests.ImageServiceTests;

[TestClass]
public class ImageServiceTagsNoPatron : ImageServiceTagsBase
{
    // Start the test with a platform-info that has WebP support
    protected override void SetupServices(IServiceCollection services)
    {
        base.SetupServices(services);
        services.AddTransient<IPlatformInfo, TestPlatformNotPatron>();
    }

    protected override bool TestModeImg => false;


    [DataRow(SrcJpgNone, SrcSetNone, "No Src Set")]
    [DataRow(SrcJpgNone, SrcSet12, "With Src Set 1,2, no patron")]
    [DataTestMethod]
    public void SourceTagsMultiTests(string expected, string variants, string name) 
        => SourceTagsMultiTest(expected, variants, name);


    [DataRow(SrcJpgNone, SrcSetNone, true, "No Src Set, in-pic")]
    [DataRow(SrcJpgNone, SrcSetNone, false, "No Src Set, in-settings")]
    [DataRow(SrcJpgNone, SrcSet12, true, "With Src Set 1,2, in-pic")]
    [DataRow(SrcJpgNone, SrcSet12, false, "With Src Set 1,2, in-settings")]
    [DataTestMethod]
    public void PictureTags(string expected, string variants, bool inPicTag, string name) 
        => PictureTagInner(expected, variants, inPicTag, name);
}