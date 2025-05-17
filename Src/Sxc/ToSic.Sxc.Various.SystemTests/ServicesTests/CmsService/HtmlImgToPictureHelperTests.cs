using ToSic.Sxc.Services;
using ToSic.Sxc.Services.CmsService.Internal;
using Xunit.Abstractions;
using ExecutionContext = ToSic.Sxc.Code.Internal.ExecutionContext;

namespace ToSic.Sxc.ServicesTests.CmsService;

public class HtmlImgToPictureHelperTests(ExecutionContext executionContext, /*needed to inject into CodeApiSvc*/ IImageService imageSvc, ITestOutputHelper output)
{
    /// <summary>
    /// Swap the image service to one which doesn't know about the app (so it won't get settings etc.)
    /// </summary>
    private void InitCodeApiSvc()
        => executionContext.ReplaceServiceInCache(imageSvc);

    /// <summary>
    /// Must get service through executionContext, because the class is internal & it needs to have a parent CodeApiService for sub-dependencies
    /// </summary>
    private HtmlImgToPictureHelper GetHtmlImgToPictureHelper()
        => executionContext.GetService<HtmlImgToPictureHelper>();

    // needs a lot more tests, such as with / without paths, etc.
    [Theory, MemberData(nameof(DataForImgConversionTest.ImageConversions), MemberType = typeof(DataForImgConversionTest))]
    public void Test(ImgConversionTest conversion)
    {
        InitCodeApiSvc();
        var parser = GetHtmlImgToPictureHelper();

        var folder = DataForCmsServiceTests.GenerateFolderWithTestPng();

        var result = parser
            .ConvertImgToPicture(conversion.Original, folder, null)
            .ToString();

        NotNull(result);

        output.WriteLine($"Original: {conversion.Original}");
        output.WriteLine($"Expected: {conversion.Expected}");
        output.WriteLine($"Result: {result}");

        Equal(conversion.Expected, result);
    }

}