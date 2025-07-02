using ToSic.Sxc.Mocks;
using ToSic.Sxc.Services;
using Xunit.Abstractions;

namespace ToSic.Sxc.ServicesTests.ImageServiceTests;

// Start the test with a platform-info that has no patron
[Startup(typeof(StartupMockExecutionContext))]
public class ImageServiceTagsImgNoPatron(ExecutionContextMock executionContext, ITestOutputHelper output)
    : ImageServiceTagsImgBase(executionContext.GetService<IImageService>(reuse: true), output, new ScenarioBasic()),
        IClassFixture<DoFixtureStartup<ScenarioBasic>>
{
    protected override bool TestModeImg => true;

    [Theory]
    [InlineData(ImgTagJpgNone, SrcSetNone, null, "No Src Set")]
    [InlineData(ImgTagJpgNone, SrcSet12, null, "With Src Set 1,2 - non patrons just like no srcset")]
    [InlineData(ImgTagJpgNoneF05, SrcSetNone, 0.5, "No Src Set, factor 0.5")]
    public override void ImageTagMultiTest(string expected, string variants, object factor, string testName) 
        => base.ImageTagMultiTest(expected, variants, factor, testName);

    [Theory]
    [InlineData(null, SrcSet12, "With Src Set 1,2, no patron")]
    [InlineData(null, SrcSetNone, "No Src Set, no patron")]
    public override void ImageSrcSetMultiTest(string expected, string variants, string testName) 
        => base.ImageSrcSetMultiTest(expected, variants, testName);
}