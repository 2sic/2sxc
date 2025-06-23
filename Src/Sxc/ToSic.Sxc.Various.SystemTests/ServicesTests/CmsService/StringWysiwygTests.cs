using ToSic.Eav.Data.Build;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.CmsService.Internal;
using Xunit.Abstractions;
using ExecutionContext = ToSic.Sxc.Code.Internal.ExecutionContext;

namespace ToSic.Sxc.ServicesTests.CmsService;

public class StringWysiwygTests(
    ExecutionContext executionContext,
    ContentTypeFactory contentTypeFactory,
    DataForCmsServiceTests dataForCmsTests,
    ICodeDataFactory cdf,
    IImageService imageSvc, // needed to inject into CodeApiSvc
    ITestOutputHelper output
    )
{
    /// <summary>
    /// Swap the image service to one which doesn't know about the app (so it won't get settings etc.)
    /// </summary>
    private void InitCodeApiSvc() => executionContext.ReplaceServiceInCache(imageSvc);

    /// <summary>
    /// Must get service through codeApiSvc, because the class is internal & it needs to have a parent CodeApiService for sub-dependencies
    /// </summary>
    private CmsServiceStringWysiwyg GetStringWysiwygParser() => executionContext.GetService<CmsServiceStringWysiwyg>();

    // TODO: needs a lot more tests, such as with / without paths, etc.

    /// <summary>
    /// This test is almost identical to the <see cref="HtmlImgToPictureHelperTests"/> but going through one more layer of objects.
    /// The primary test should be there, this should just confirm that these simple setups create the same result.
    /// </summary>
    /// <param name="conversion"></param>
    [Theory, MemberData(nameof(DataForImgConversionTest.ImageConversions), MemberType = typeof(DataForImgConversionTest))]
    public void ImageTagOnlyHasSameResultAsHtmlImgToPictureHelper(ImgConversionTest conversion)
    {
        InitCodeApiSvc();
        var parser = GetStringWysiwygParser();

        var ctWithHtmlField = contentTypeFactory.Create(typeof(MockHtmlContentType));

        var attribute = ctWithHtmlField.Attributes
            .First(a => a.Name == nameof(MockHtmlContentType.SomeHtml));

        var data = dataForCmsTests.TstDataEntity("hello", conversion.Original, ctWithHtmlField);
        var typed = cdf.AsItem(data, new() { ItemIsStrict = true });
        var field = typed.Field(nameof(MockHtmlContentType.SomeHtml));

        var folder = DataForCmsServiceTests.GenerateFolderWithTestPng();
        parser.Init(field, ctWithHtmlField, attribute, folder, false, null);

        var result = parser.HtmlForStringAndWysiwyg(conversion.Original);

        NotNull(result);

        output.WriteLine($"Expected: {conversion.Expected}");
        output.WriteLine($"Result: {result.Contents}");

        Equal(conversion.Expected, result.Contents);
    }



}