using ToSic.Sxc.Mocks;
using ToSic.Sxc.Services.CmsService.Internal;
using Xunit.Abstractions;

namespace ToSic.Sxc.ServicesTests.CmsService;

public class HtmlImgToPictureHelperTests(ExecutionContextMock exCtxMock, ITestOutputHelper output)
    // Needs fixture to load the Primary App
    : IClassFixture<DoFixtureStartup<ScenarioBasic>>
{
    /// <summary>
    /// Must get service through executionContext, because the class is internal & it needs to have a parent CodeApiService for sub-dependencies
    /// </summary>
    private HtmlImgToPictureHelper GetHtmlImgToPictureHelper()
        => exCtxMock.GetService<HtmlImgToPictureHelper>();

    // needs a lot more tests, such as with / without paths, etc.
    [Theory, MemberData(nameof(DataForImgConversionTest.ImageConversions), MemberType = typeof(DataForImgConversionTest))]
    public void Test(ImgConversionTest conversion)
    {
        var parser = GetHtmlImgToPictureHelper();

        var folder = DataForCmsServiceTests.GenerateFolderWithTestPng();

        // These are necessary, as otherwise the test will automatically look up the "Content" settings for image resizing
        // which would result in a different result.
        // TODO: ALSO create a test which uses null, and expects the proper resized with default settings
        var fakeEmptySettings = new object();

        var result = parser
            .ConvertImgToPicture(conversion.Original, folder, fakeEmptySettings)
            .ToString();

        NotNull(result);

        output.WriteLine($"Original: {conversion.Original}");
        output.WriteLine($"Expected: {conversion.Expected}");
        output.WriteLine($"Result: {result}");

        Equal(conversion.Expected, result);
    }

}