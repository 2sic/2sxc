using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Internal.Loaders;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services.Internal;
using CodeDataFactory = ToSic.Sxc.Data.Internal.CodeDataFactory;

namespace ToSic.Sxc.ServicesTests.CmsService;

public class CmsServiceTestBase: IClassFixture<DoFixtureStartup<ScenarioFullPatrons>>
{
    public const int AppId = -1;
    public const string SomeTextField = "SomeText";
    public const string SomeHtmlField = "SomeHtml";

    public CmsServiceTestBase(EavSystemLoader eavSystemLoader, IAppReaderFactory appStates, CodeDataFactory cdf, DataBuilder dataBuilder, ICmsService cmsService, ContentTypeFactory contentTypeFactory)
    {
        eavSystemLoader.StartUp();
        eavSystemLoader.LoadLicenseAndFeatures();

        var app = appStates.GetSystemPresetTac();
        //TstDataContentType = app.GetContentType("TstData");
        //if (TstDataContentType == null)
        //    throw new("TstData content type not found. Probably JSON is missing.");

        TstDataContentType = contentTypeFactory.Create(typeof(MockHtmlContentType));

        Cdf = cdf;
        _dataBuilder = dataBuilder;
        _cmsService = cmsService;
    }
    public readonly CodeDataFactory Cdf;
    private readonly DataBuilder _dataBuilder;
    private readonly ICmsService _cmsService;
    public readonly IContentType TstDataContentType;

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
        => Equal(expectedHtml, CmsServiceShow(html, "").ToString());

    public IHtmlTag CmsServiceShow(string someHtmlValue, string? container = default)
    {
        const string someTextValue = "Just Basic Text";
        var entity = TstDataEntity(someTextValue, someHtmlValue, TstDataContentType);
        var dynamicEntity = DynEntStrict(entity);
        var dynamicField = dynamicEntity.Field(SomeHtmlField);
        return _cmsService.Html(dynamicField, container: container);
    }

    public IEntity TstDataEntity(string text = "", string html = "", IContentType? contentType = null)
    {
        var values = new Dictionary<string, object>
        {
            { SomeTextField, text },
            { SomeHtmlField, html }
        };
        return _dataBuilder.CreateEntityTac(appId: AppId, entityId: 1, contentType: contentType, values: values, titleField: SomeTextField);
    }

    public DynamicEntity DynEntStrict(IEntity? entity = null) => Cdf.AsDynamic(entity, true);

}