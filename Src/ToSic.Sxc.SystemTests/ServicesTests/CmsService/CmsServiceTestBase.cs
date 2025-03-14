using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Mocks;
using ToSic.Eav.Integration;
using ToSic.Eav.Internal.Loaders;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services.Internal;
using CodeDataFactory = ToSic.Sxc.Data.Internal.CodeDataFactory;

namespace ToSic.Sxc.Tests.ServicesTests.CmsService;

[Startup(typeof(StartupSxcCoreOnly))]
public class CmsServiceTestBase // : TestBaseSxcDb
{
    public class Startup: StartupSxcCoreOnly
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddAppLoader();
            services.TryAddTransient<IValueConverter, MockValueConverter>();
            base.ConfigureServices(services);
        }
    }

    public const int AppId = -1;
    public const string SomeTextField = "SomeText";
    public const string SomeHtmlField = "SomeHtml";

    public CmsServiceTestBase(EavSystemLoader eavSystemLoader, IAppReaderFactory appStates, CodeDataFactory cdf, DataBuilder dataBuilder, ICmsService cmsService)
    {
        //var eavSystemLoader = GetService<EavSystemLoader>();
        eavSystemLoader.StartUp();
        eavSystemLoader.LoadLicenseAndFeatures();

        //var appStates = GetService<IAppReaderFactory>();
        var app = appStates.GetSystemPresetTac();
        TstDataContentType = app.GetContentType("TstData");
        if (TstDataContentType == null)
            throw new("TstData content type not found. Probably JSON is missing.");
        Cdf = cdf;//GetService<CodeDataFactory>();
        _dataBuilder = dataBuilder;
        _cmsService = cmsService;
    }
    public readonly CodeDataFactory Cdf;
    private readonly DataBuilder _dataBuilder;
    private readonly ICmsService _cmsService;
    public readonly IContentType TstDataContentType;

    //protected override IServiceCollection SetupServices(IServiceCollection services)
    //{
    //    base.SetupServices(services)
    //        .AddAppLoader();
    //    // services.AddTransient<IAppLoader, AppLoader>();
    //    services.TryAddTransient<IValueConverter, MockValueConverter>();
    //    return services;
    //}

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("<p>some html</p>", "<p>some html</p>")]
    [InlineData("<img src='img.png' data-cmsid='file:1' class='wysiwyg-width1of5'>",
        "<picture><source type='' srcset='http://mock.converted/file:1'><img src='http://mock.converted/file:1' class='wysiwyg-width1of5'></picture>")]
    [InlineData("<img src='tst.jpg' data-cmsid='file:1' class='img-fluid' loading='lazy' alt='description' width='1230' height='760' style='width:auto;'>",
        "<picture><source type='' srcset='http://mock.converted/file:1?w=1230'><img src='http://mock.converted/file:1?w=1230' alt='description' class='img-fluid' loading='lazy' height='760' style='width:auto;'></picture>")]
    public void BasicCmsService(string html, string expectedHtml)
        => Equal(expectedHtml, CmsServiceShow(html).ToString());


    public IEntity TstDataEntity(string text = "", string html = "", IContentType contentType = null)
    {
        var values = new Dictionary<string, object>()
        {
            {SomeTextField, text},
            {SomeHtmlField, html}
        };
        return _dataBuilder.CreateEntityTac(appId: AppId, entityId: 1, contentType: contentType, values: values, titleField: SomeTextField);
    }

    public DynamicEntity DynEntStrict(IEntity entity = null) => Cdf.AsDynamic(entity, true);

    public IHtmlTag CmsServiceShow(string someHtmlValue)
    {
        const string someTextValue = "Just Basic Text";
        var entity = TstDataEntity(someTextValue, someHtmlValue, TstDataContentType);
        var dynamicEntity = DynEntStrict(entity);
        var dynamicField = dynamicEntity.Field(SomeHtmlField);
        //var cmsService = GetService<ICmsService>();
        return _cmsService.Html(dynamicField);
    }
}