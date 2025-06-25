using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.ServicesTests.CmsService;

public class CmsServiceTests(ICodeDataFactory cdf, ICmsService cmsService, DataForCmsServiceTests dataForCmsTests, ContentTypeFactory contentTypeFactory)
    : IClassFixture<DoFixtureStartup<ScenarioFullPatronsWithDb>>
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


    public IHtmlTag CmsServiceShow(string someHtmlValue, string? container = default, IContentType? contentType = default)
    {
        const string someTextValue = "Just Basic Text";
        var entity = dataForCmsTests.TstDataEntity(someTextValue, someHtmlValue, contentType);
        var dynamicEntity = DynEntStrict(entity);
        var dynamicField = dynamicEntity.Field(DataForCmsServiceTests.SomeHtmlField);
        return cmsService.Html(dynamicField, container: container);
    }

    public IDynamicEntity DynEntStrict(IEntity? entity = null) => cdf.AsDynamic(entity, new() { ItemIsStrict = true });

}