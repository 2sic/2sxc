using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps.Internal;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services.Cache;
using ToSic.Sxc.Services.CmsService.Internal;
using ToSic.Sxc.Services.DataServices;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Services.OutputCache;
using ToSic.Sxc.Services.Templates;
using ToSic.Sxc.Web.Internal.PageService;

namespace ToSic.Sxc.Services;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class SxcServicesStartup
{
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcServices(this IServiceCollection services)
    {
        // new in v12.02 - PageService & Page Features
        services.TryAddTransient<IPageService, PageService>();  // must be unique per module where it's used

        //services.AddObsoleteServicesAndKits();

        // WIP 12.05 - json converter
        services.TryAddTransient<IJsonService, JsonService>();
        services.TryAddTransient<IJsonServiceInternal, JsonService>();
        services.TryAddTransient<ConvertValueService>();
        services.TryAddTransient<ConvertForCodeService>();
        services.TryAddTransient<IConvertService, ConvertService>();
        services.TryAddTransient<IConvertService16, ConvertService16>();

        // New 12.05: SecureData
        services.TryAddTransient<ISecureDataService, SecureDataService>();

        // 13 - ToolbarService & IFeaturesService
        services.TryAddTransient<IToolbarService, ToolbarService>();    // New 13.00
        services.TryAddTransient<IFeaturesService, FeaturesService>();  // New 13.01
        //services.TryAddTransient<IImageService, ImageService>();
        //services.TryAddTransient<IEditService, EditService>();
        services.TryAddTransient<ResizeDimensionGenerator>();

        // WIP v14
        services.TryAddTransient<IAdamService, AdamService>();

        // V15
        services.TryAddScoped<IModuleService, ModuleService>(); // Must be scoped & shared on the module
        services.TryAddTransient<ITurnOnService, TurnOnService>();
        services.TryAddTransient<ICmsService, CmsService.Internal.CmsService>();
        services.TryAddTransient<CmsServiceStringWysiwyg>();
        services.TryAddTransient<CmsServiceImageExtractor>();
        services.TryAddTransient<IDataService, DataService>();

        // 19.03.03 - CmsService improving SoC
        services.TryAddTransient<HtmlImgToPictureHelper>();
        services.TryAddTransient<HtmlInnerContentHelper>();
        services.TryAddTransient<IOutputCacheService, OutputCacheService>();    // WIP v19.03.03, not official ATM
        services.TryAddTransient<OutputCacheService>();                         // WIP v19.03.03, not official ATM

        //// v15 CustomDataSources - just the dependencies needed
        //services.TryAddTransient<DataSource16.MyServices>();

        // v16 AsConverter
        //services.TryAddTransient<CodeDataFactory>(sp => ActivatorUtilities.CreateInstance<CodeDataFactory>(sp));
        //services.TryAddTransient<CodeDataFactory>();
        //services.TryAddTransient<ICodeDataFactory, CodeDataFactory>();
        //services.TryAddTransient<CodeDataServices>();

        // Kits v14 - v16
        services.TryAddTransient<ServiceKit>();
        services.TryAddTransient<ServiceKit14>();
        services.TryAddTransient<ServiceKit16>();
        //services.TryAddTransient<ServiceKitLight16>();

        // Lookup Service - WIP v17
        services.TryAddTransient<ITemplateService, TemplateService>();

        // Cache Service - WIP v17
        services.TryAddTransient<ICacheService, CacheService>();

        // v17.01
        services.TryAddTransient<IUserService, UserService>();

        services.TryAddTransient<IKeyService, KeyService>();

        services.AddSxcServicesFallbacks();
        services.ExternalConfig();

        return services;
    }

    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection ExternalConfig(this IServiceCollection services)
    {
        // new v15
        services.TryAddTransient<GoogleMapsSettings>();
        return services;
    }


    [ShowApiWhenReleased(ShowApiMode.Never)]
    public static IServiceCollection AddSxcServicesFallbacks(this IServiceCollection services)
    {
        // v12.05 - LinkServiceUnknown - for testing etc.
        services.TryAddTransient<ILinkService, LinkServiceUnknown>();

        // v12.05
        services.TryAddTransient<ISystemLogService, SystemLogServiceUnknown>();

        // v12.05
        services.TryAddTransient<IMailService, MailServiceUnknown>();

        return services;
    }


}