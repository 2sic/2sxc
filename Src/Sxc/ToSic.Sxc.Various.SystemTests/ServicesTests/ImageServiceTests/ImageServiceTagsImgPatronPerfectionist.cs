using ToSic.Sxc.Services;
using Xunit.Abstractions;

namespace ToSic.Sxc.ServicesTests.ImageServiceTests;

//// Start the test with a platform-info that has WebP support
[Startup(typeof(StartupSxcWithDbPatronPerfectionist))]
public class ImageServiceTagsImgPatronPerfectionist(IImageService imgSvc, ITestOutputHelper output)
    : ImageServiceTagsImgBase(imgSvc, output), IClassFixture<DoFixtureStartup<ScenarioFullPatronsWithDb>>
{
    protected override bool TestModeImg => true;

    [Theory]
    [InlineData(ImgTagJpgNone, SrcSetNone, null, "No Src Set")]
    [InlineData(ImgTagJpg12, SrcSet12, null, "With Src Set 1,2")]
    [InlineData(ImgTagJpgNoneF05, SrcSetNone, 0.5, "No Src Set, factor 0.5")]
    public override void ImageTagMultiTest(string expected, string variants, object factor, string testName) 
        => base.ImageTagMultiTest(expected, variants, factor, testName);

    [Theory]
    [InlineData(Img120x24x + ",\n" + Img240x48x, SrcSet12, "With Src Set 1,2")]
    [InlineData(null, SrcSetNone, "No Src Set")]
    public override void ImageSrcSetMultiTest(string expected, string variants, string testName) 
        => base.ImageSrcSetMultiTest(expected, variants, testName);
}