using ToSic.Eav.Data.Build;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.CmsService.Internal;
using Xunit.Abstractions;

namespace ToSic.Sxc.ServicesTests.CmsService;

public class StringWysiwygTests(
    CodeApiService codeApiSvc,
    ContentTypeFactory contentTypeFactory,
    CmsServiceTestData cmsTestData,
    CodeDataFactory cdf,
    IImageService imageSvc, // needed to inject into CodeApiSvc
    ITestOutputHelper output
    )
{
    /// <summary>
    /// Must get the desired service through codeApiSvc, because
    /// - the class is internal
    /// - it needs to have a parent CodeApiService for sub-dependencies
    /// </summary>
#if NETCOREAPP
    [field: System.Diagnostics.CodeAnalysis.AllowNull, System.Diagnostics.CodeAnalysis.MaybeNull]
#endif
    private CmsServiceStringWysiwyg StringWysiwygParser => field ??= new Func<CmsServiceStringWysiwyg>(()=>
    {
        // Swap the image service to one which doesn't know about the app (so it won't get settings etc.)
        codeApiSvc.ReplaceServiceInCache(imageSvc);
        return codeApiSvc.GetService<CmsServiceStringWysiwyg>();
    })();

    // needs a lot more tests, such as with / without paths, etc.

    public static TheoryData<ImgConversionTest> ImageConversions =>
    [
        new() { AName = "Basic img.png" },
        new() { AName = "Basic with path", ImgNameAndFolder = "some-path/img.png" },
        new() { AName = "jpg with path", ImgNameAndFolder = "some-path/img.jpg", MimeType = "image/jpeg" },
        new()
        {
            AName = "Extensive with tst.jpg",
            OriginalHtml = "<img src='tst.jpg' data-cmsid='file:tst.jpg' class='img-fluid' loading='lazy' alt='description' width='1230' height='760' style='width:auto;'>",
            Expected = "<picture class='img-fluid'><source type='image/jpeg' srcset='tst.jpg?w=1230'><img src='tst.jpg?w=1230' loading='lazy' height='760' alt='description' class='img-fluid' style='width:auto;'></picture>",
        }
    ];

    [Theory]
    [MemberData(nameof(ImageConversions))]
    public void Test(ImgConversionTest conversion)
    {
        var ctWithHtmlField = contentTypeFactory.Create(typeof(MockHtmlContentType));

        var attribute = ctWithHtmlField.Attributes
            .First(a => a.Name == nameof(MockHtmlContentType.SomeHtml));

        var data = cmsTestData.TstDataEntity("hello", conversion.OriginalHtml, ctWithHtmlField);
        var typed = cdf.AsItem(data);
        var field = typed.Field(nameof(MockHtmlContentType.SomeHtml));

        var folder = CmsServiceTestData.GenerateFolderWithTestPng();
        StringWysiwygParser.Init(field, ctWithHtmlField, attribute, folder, false, null);

        var result = StringWysiwygParser.HtmlForStringAndWysiwyg(conversion.OriginalHtml);

        NotNull(result);

        output.WriteLine($"Expected: {conversion.Expected}");
        output.WriteLine($"Result: {result.Contents}");

        Equal(conversion.Expected, result.Contents);
    }



}