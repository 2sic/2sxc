using ToSic.Eav.Data.Build;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Mocks;
using ToSic.Sxc.Services.Cms.Sys;
using Xunit.Abstractions;

namespace ToSic.Sxc.ServicesTests.CmsService;

public class StringWysiwygTests(
    ExecutionContextMock executionContext,
    ContentTypeFactory contentTypeFactory,
    DataForCmsServiceTests dataForCmsTests,
    ICodeDataFactory cdf,
    ITestOutputHelper output
    )
    // Needs fixture to load the Primary App
    : IClassFixture<DoFixtureStartup<ScenarioBasic>>
{

    // TODO: needs a lot more tests, such as with / without paths, etc.

    /// <summary>
    /// This test is almost identical to the <see cref="HtmlImgToPictureHelperTests"/> but going through one more layer of objects.
    /// The primary test should be there, this should just confirm that these simple setups create the same result.
    /// </summary>
    /// <param name="conversion"></param>
    [Theory, MemberData(nameof(DataForImgConversionTest.ImageConversions), MemberType = typeof(DataForImgConversionTest))]
    public void ImageTagOnlyHasSameResultAsHtmlImgToPictureHelper(ImgConversionTest conversion)
    {
        // Must get service through codeApiSvc, because the class is internal & it needs to have a parent CodeApiService for sub-dependencies
        var parser = executionContext.GetService<CmsServiceStringWysiwyg>();

        var ctWithHtmlField = contentTypeFactory.CreateTac<MockHtmlContentType>();

        var attribute = ctWithHtmlField.Attributes
            .First(a => a.Name == nameof(MockHtmlContentType.SomeHtml));

        var data = dataForCmsTests.TstDataEntity("hello", conversion.Original, ctWithHtmlField);
        var typed = cdf.AsItem(data, new() { ItemIsStrict = true });
        var field = typed.Field(nameof(MockHtmlContentType.SomeHtml))!;

        var folder = DataForCmsServiceTests.GenerateFolderWithTestPng();

        // These are necessary, as otherwise the test will automatically look up the "Content" settings for image resizing
        // which would result in a different result.
        // TODO: ALSO create a test which uses null, and expects the proper resized with default settings
        var fakeEmptySettings = new object();

        parser.Init(field, ctWithHtmlField, attribute, folder, false, fakeEmptySettings);

        var result = parser.HtmlForStringAndWysiwyg(conversion.Original);

        NotNull(result);

        output.WriteLine($"Expected: {conversion.Expected}");
        output.WriteLine($"Result: {result.Contents}");

        Equal(conversion.Expected, result.Contents);
    }



}