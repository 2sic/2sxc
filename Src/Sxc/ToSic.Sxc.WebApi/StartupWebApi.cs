using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Sys.Insights;
using ToSic.Eav.WebApi.Sys.Admin;
using ToSic.Eav.WebApi.Sys.ImportExport;
using ToSic.Sxc.Apps.Internal;
using ToSic.Sxc.Apps.Internal.Assets;
using ToSic.Sxc.Apps.Sys.EditAssets;
using ToSic.Sxc.Backend.Adam;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sxc.Backend.Admin.AppFiles;
using ToSic.Sxc.Backend.Admin.Query;
using ToSic.Sxc.Backend.App;
using ToSic.Sxc.Backend.AppStack;
using ToSic.Sxc.Backend.Cms;
using ToSic.Sxc.Backend.ContentBlocks;
using ToSic.Sxc.Backend.Context;
using ToSic.Sxc.Backend.ImportExport;
using ToSic.Sxc.Backend.InPage;
using ToSic.Sxc.Backend.SaveHelpers;
using ToSic.Sxc.Backend.Sys;
using ToSic.Sxc.Backend.Usage;
using ToSic.Sxc.Backend.Views;
using ToSic.Sxc.Render.Sys.Output;
using ToSic.Sxc.Render.Sys.ResourceExtractor;
using ToSic.Sxc.WebApi.Sys;
using ToSic.Sxc.WebApi.Sys.ExternalLinks;

namespace ToSic.Sxc.Backend;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class StartupWebApi
{
    public static IServiceCollection AddSxcWebApi(this IServiceCollection services)
    {
        // WIP - objects which are not really final
        services.TryAddTransient<ExternalLinksService>();

        // These are usually replaced by the target platform
        services.TryAddTransient<IBlockResourceExtractor, BlockResourceExtractorUnknown>();
            
        // Real Controllers

        // Backends
        services.TryAddTransient<AppsBackend>();
        services.TryAddTransient<EntityBackend>();
        services.TryAddTransient<EditLoadBackend>();
        services.TryAddTransient<EditLoadPrefetchHelper>();
        services.TryAddTransient<EditLoadSettingsHelper>();
        services.TryAddTransient<EditSaveBackend>();
        services.TryAddTransient<SaveSecurity>();
        services.TryAddTransient<AppViewPickerBackend>();
        services.TryAddTransient<ContentBlockBackend>();
        services.TryAddTransient<UsageBackend>();

        // Internal API helpers
        services.TryAddTransient<AppContent>();
        services.TryAddTransient<SxcPagePublishing>();
        services.TryAddTransient<ExportApp>();
        services.TryAddTransient<ImportApp>();
        services.TryAddTransient<ImportContent>();
        services.TryAddTransient<ExportContent>();
        services.TryAddTransient<ResetApp>();
        services.TryAddTransient<AppStackBackend>();
        services.TryAddTransient<AppFolderLookupForWebApi>();
        services.TryAddTransient<ViewsExportImport>();

        // Small WebApi Helpers
        services.TryAddTransient<ContentGroupList>();

        // js context / UI
        services.TryAddTransient<IUiContextBuilder, UiContextBuilderUnknown>();
        services.TryAddTransient<UiContextBuilderBase.MyServices>();
            
        // Helpers
        services.TryAddTransient<ImpExpHelpers>();

        // Adam shared code across the APIs
        services.TryAddTransient<AdamCode>();

        // New v13 - try to reduce Dnn/Oqtane code to the max, by creating ControllerReal objects which do everything
        services.TryAddTransient(typeof(AdamControllerReal<>));
        services.TryAddTransient<AppFilesControllerReal>();
        services.TryAddTransient<IAppExplorerControllerDependency, AppFilesControllerReal>();
        services.TryAddTransient<QueryControllerReal>();
        services.TryAddTransient<AppControllerReal>();
        services.TryAddTransient<AppPartsControllerReal>();
        services.TryAddTransient<DialogControllerReal>();
        services.TryAddTransient<TypeControllerReal>();
        services.TryAddTransient<ViewControllerReal>();
        services.TryAddTransient<AppDataControllerReal>();
        services.TryAddTransient<AppQueryControllerReal>();
        services.TryAddTransient<ContentGroupControllerReal>();
        services.TryAddTransient<EditControllerReal>();
        services.TryAddTransient<HistoryControllerReal>();
        services.TryAddTransient<ListControllerReal>();
        services.TryAddTransient<InstallControllerReal>();
        services.TryAddTransient<BlockControllerReal>();
        services.TryAddTransient<CodeControllerReal>();
        services.TryAddTransient<DataControllerReal>();

        services.TryAddTransient<AssetTemplates>();

        services.AddLoadSettingsProviders();

        services.AddSxcInsights();

        services.AddFallbacks();

        return services;
    }

    public static IServiceCollection AddLoadSettingsProviders(this IServiceCollection services)
    {
        services.AddTransient<ILoadSettingsProvider, LoadSettingsForGpsDefaults>();
        services.AddTransient<ILoadSettingsProvider, LoadSettingsForContentType>();
        services.AddTransient<ILoadSettingsProvider, LoadSettingsApiKeys>();
        services.AddTransient<ILoadSettingsContentTypesProvider, LoadSettingsForPickerSources>();
        return services;
    }

    public static IServiceCollection AddAdamWebApi<TFolder, TFile>(this IServiceCollection services)
    {
        // Adam Controllers etc.
        services.TryAddTransient(typeof(HyperlinkBackend));
        services.TryAddTransient(typeof(AdamItemDtoMaker<,>));
        services.TryAddTransient(typeof(AdamItemDtoMaker<,>.MyServices));

        // Default `int` implementation, the platform must specify a different type before this if it needs another identity type
        services.TryAddTransient<IAdamItemDtoMaker, AdamItemDtoMaker<TFolder, TFile>>();

        // Prefetch helper so it can be used in the Edit CMS Load
        services.TryAddTransient<IAdamPrefetchHelper, AdamPrefetchHelper>();

        return services;
    }

    public static IServiceCollection AddSxcInsights(this IServiceCollection services)
    {
        services.AddTransient<IInsightsProvider, InsightsAppCodeOverview>();
        services.AddTransient<IInsightsProvider, InsightsAppCodeBuild>();
        services.AddTransient<IInsightsProvider, InsightsLightSpeed>();
        return services;
    }

    public static IServiceCollection AddFallbacks(this IServiceCollection services)
    {
        services.TryAddTransient<IWebApiContextBuilder, WebApiContextBuilderUnknown>();
        return services;
    }
        
}