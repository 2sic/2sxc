﻿using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services.Internal;
using CodeDataFactory = ToSic.Sxc.Data.Internal.CodeDataFactory;

namespace ToSic.Sxc.ServicesTests.CmsService;

public class CmsServiceTests(CodeDataFactory cdf, ICmsService cmsService, DataForCmsServiceTests dataForCmsTests, ContentTypeFactory contentTypeFactory)
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


    public IHtmlTag CmsServiceShow(string someHtmlValue, string? container = default, IContentType? contentType = default)
    {
        const string someTextValue = "Just Basic Text";
        var entity = dataForCmsTests.TstDataEntity(someTextValue, someHtmlValue, contentType);
        var dynamicEntity = DynEntStrict(entity);
        var dynamicField = dynamicEntity.Field(DataForCmsServiceTests.SomeHtmlField);
        return cmsService.Html(dynamicField, container: container);
    }

    public DynamicEntity DynEntStrict(IEntity? entity = null) => cdf.AsDynamic(entity, true);

}