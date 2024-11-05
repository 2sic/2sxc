using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Eav.Context;
using ToSic.Testing.Shared.Platforms;

namespace ToSic.Sxc.Tests.ServicesTests.ImageServiceTests;

[TestClass]
public class ImageServiceTagsImgPatronPerfectionist : ImageServiceTagsImgBase
{
    // Start the test with a platform-info that has WebP support
    protected override void SetupServices(IServiceCollection services)
    {
        base.SetupServices(services);
        services.AddTransient<IPlatformInfo, TestPlatformPatronPerfectionist>();
    }

    protected override bool TestModeImg => true;

    [DataRow(ImgTagJpgNone, SrcSetNone, null, "No Src Set")]
    [DataRow(ImgTagJpg12, SrcSet12, null, "With Src Set 1,2")]
    [DataRow(ImgTagJpgNoneF05, SrcSetNone, 0.5, "No Src Set, factor 0.5")]
    [DataTestMethod]
    public new void ImageTagMultiTest(string expected, string variants, object factor, string testName) 
        => base.ImageTagMultiTest(expected, variants, factor, testName);

    [DataRow(Img120x24x + ",\n" + Img240x48x, SrcSet12, "With Src Set 1,2")]
    [DataRow(null, SrcSetNone, "No Src Set")]
    [DataTestMethod]
    public new void ImageSrcSetMultiTest(string expected, string variants, string testName) 
        => base.ImageSrcSetMultiTest(expected, variants, testName);
}