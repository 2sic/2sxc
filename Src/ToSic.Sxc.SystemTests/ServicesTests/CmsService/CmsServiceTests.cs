using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services.Internal;
using CodeDataFactory = ToSic.Sxc.Data.Internal.CodeDataFactory;

namespace ToSic.Sxc.ServicesTests.CmsService;

public class CmsServiceTests(CodeDataFactory cdf, ICmsService cmsService, CmsServiceTestData cmsTestData, ContentTypeFactory contentTypeFactory)
    : IClassFixture<DoFixtureStartup<ScenarioFullPatrons>>
{
#if NETCOREAPP
    [field: System.Diagnostics.CodeAnalysis.AllowNull, System.Diagnostics.CodeAnalysis.MaybeNull]
#endif
    public IContentType TstDataContentType => field ??= contentTypeFactory.Create(typeof(MockHtmlContentType));

    [Theory]
    [InlineData(null, "", "")]
    [InlineData(null, null, "<div></div>")]
    [InlineData("", "", "")]
    [InlineData("", null, "<div></div>")]
    [InlineData("<p>some html</p>", "", "<p>some html</p>")]
    [InlineData("<p>some html</p>", null, "<div><p>some html</p></div>")]
    public void BasicCmsService(string html, string container, string expectedHtml)
        => Equal(expectedHtml, CmsServiceShow(html, container).ToString());


    [Theory]
    [InlineData("<img src='img.png' data-cmsid='file:1' class='wysiwyg-width1of5'>",
        "<picture><source type='' srcset='http://mock.converted/file:1'><img src='http://mock.converted/file:1' class='wysiwyg-width1of5'></picture>")]
    [InlineData("<img src='tst.jpg' data-cmsid='file:1' class='img-fluid' loading='lazy' alt='description' width='1230' height='760' style='width:auto;'>",
        "<picture><source type='' srcset='http://mock.converted/file:1?w=1230'><img src='http://mock.converted/file:1?w=1230' alt='description' class='img-fluid' loading='lazy' height='760' style='width:auto;'></picture>")]
    public void ImgBecomesPicture(string html, string expectedHtml)
        => Equal(expectedHtml, CmsServiceShow(html, "", contentType: TstDataContentType).ToString());

    public IHtmlTag CmsServiceShow(string someHtmlValue, string? container = default, IContentType contentType = default)
    {
        const string someTextValue = "Just Basic Text";
        var entity = cmsTestData.TstDataEntity(someTextValue, someHtmlValue, contentType);
        var dynamicEntity = DynEntStrict(entity);
        var dynamicField = dynamicEntity.Field(CmsServiceTestData.SomeHtmlField);
        return cmsService.Html(dynamicField, container: container);
    }

    public DynamicEntity DynEntStrict(IEntity? entity = null) => cdf.AsDynamic(entity, true);

}