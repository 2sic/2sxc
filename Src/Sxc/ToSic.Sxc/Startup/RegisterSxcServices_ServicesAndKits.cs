using Custom.DataSource;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps.Internal;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Edit.EditService;
using ToSic.Sxc.Edit.Toolbar;
using ToSic.Sxc.Edit.Toolbar.Internal;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Cache;
using ToSic.Sxc.Services.CmsService;
using ToSic.Sxc.Services.DataServices;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Services.Templates;
using ToSic.Sxc.Web.Internal.ContentSecurityPolicy;
using ToSic.Sxc.Web.Internal.PageService;
using CodeDataFactory = ToSic.Sxc.Data.Internal.CodeDataFactory;

namespace ToSic.Sxc.Startup;

static partial class RegisterSxcServices
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static IServiceCollection AddServicesAndKits(this IServiceCollection services)
    {
        services.TryAddTransient<IContentSecurityPolicyService, ContentSecurityPolicyService>();

        // new in v12.02 - PageService & Page Features
        services.TryAddTransient<IPageService, PageService>();  // must be unique per module where it's used

        services.AddObsoleteServicesAndKits();

        services.TryAddTransient<Services.IRenderService, RenderService>();  // new 12.05
        services.TryAddTransient<RenderService.MyServices>();
        services.TryAddTransient<SimpleRenderer>();
        services.TryAddTransient<InTextContentBlockRenderer>();
#if NETFRAMEWORK
#pragma warning disable CS0618
        services.TryAddTransient<Blocks.IRenderService, RenderService>();  // Obsolete, but keep for the few apps we already released in v12
#pragma warning restore CS0618
#endif

        // WIP 12.05 - json converter
        services.TryAddTransient<IJsonService, JsonService>();
        services.TryAddTransient<IJsonServiceInternal, JsonService>();
        services.TryAddTransient<ConvertValueService>();
        services.TryAddTransient<ConvertForCodeService>();
        services.TryAddTransient<IConvertService, ConvertService>();
        services.TryAddTransient<IConvertService16, ConvertService16>();

        // New 12.05: SecureData
        services.TryAddTransient<ISecureDataService, SecureDataService>();

        // v13 DynamicCodeService
        services.TryAddTransient<DynamicCodeService.MyServices>();
        services.TryAddTransient<DynamicCodeService.MyScopedServices>();  // new v15
        services.TryAddTransient<IDynamicCodeService, DynamicCodeService>();

        // 13 - ToolbarService & IFeaturesService
        services.TryAddTransient<IToolbarService, ToolbarService>();    // New 13.00
        services.TryAddTransient<IFeaturesService, FeaturesService>();  // New 13.01
        services.TryAddTransient<IImageService, ImageService>();
        services.TryAddTransient<IEditService, EditService>();
        services.TryAddTransient<ResizeDimensionGenerator>();

        // v14 Toolbar Builder
        services.TryAddTransient<IToolbarBuilder, ToolbarBuilder>();
        services.TryAddTransient<ToolbarBuilder.MyServices>();
        services.TryAddTransient<ToolbarButtonDecoratorHelper>();

        // WIP v14
        services.TryAddTransient<IAdamService, AdamService>();

        // V15
        services.TryAddScoped<IModuleService, ModuleService>(); // Must be scoped & shared on the module
        services.TryAddTransient<ITurnOnService, TurnOnService>();
        services.TryAddTransient<ICmsService, CmsService>();
        services.TryAddTransient<CmsServiceStringWysiwyg>();
        services.TryAddTransient<CmsServiceImageExtractor>();
        services.TryAddTransient<IDataService, DataService>();

        // v15 CustomDataSources - just the dependencies needed
        services.TryAddTransient<DataSource16.MyServices>();

        // v16 AsConverter
        services.TryAddTransient<CodeDataFactory>(sp => ActivatorUtilities.CreateInstance<CodeDataFactory>(sp));
        services.TryAddTransient<CodeDataServices>();

        // Kits v14 - v16
        services.TryAddTransient<ServiceKit>();
        services.TryAddTransient<ServiceKit14>();
        services.TryAddTransient<ServiceKit16>();
        services.TryAddTransient<ServiceKitLight16>();

        // Lookup Service - WIP v17
        services.TryAddTransient<ITemplateService, TemplateService>();

        // Cache Service - WIP v17
        services.TryAddTransient<ICacheService, CacheService>();

        return services;
    }
}