using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps.Inspect.Sys;
using ToSic.Eav.Apps.Inspect.Sys.WebApi;
using ToSic.Eav.Apps.Install.Sys;
using ToSic.Eav.DataFormats.EavLight.Sys;
using ToSic.Eav.DataSource.DataProviders;
using ToSic.Eav.Sys.Insights;
using ToSic.Eav.Ui.Context.Sys;
using ToSic.Sxc.Apps.Extensions.Sys;
using ToSic.Sxc.Apps.Extensions.Sys.Export;
using ToSic.Sxc.Apps.Extensions.Sys.Inspect;
using ToSic.Sxc.Apps.Extensions.Sys.Install;
using ToSic.Sxc.Apps.ImportExport.Sys;
using ToSic.Sxc.Apps.Sys.DataSources;
using ToSic.Sxc.Apps.Sys.EditAssets;
using ToSic.Sxc.Backend;
using ToSic.Sxc.Backend.Adam;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sxc.Backend.Admin.AppFiles;
using ToSic.Sxc.Backend.Admin.Query;
using ToSic.Sxc.Backend.App;
using ToSic.Sxc.Backend.Cms;
using ToSic.Sxc.Backend.Cms.Load.Activities;
using ToSic.Sxc.Backend.Cms.Load.Settings;
using ToSic.Sxc.Backend.ContentBlocks;
using ToSic.Sxc.Backend.Context;
using ToSic.Sxc.Backend.ImportExport;
using ToSic.Sxc.Backend.InPage;
using ToSic.Sxc.Backend.SaveHelpers;
using ToSic.Sxc.Backend.Sys;
using ToSic.Sxc.Backend.Usage;
using ToSic.Sxc.Backend.Views;
using ToSic.Sxc.Data.Sys.Convert;
using ToSic.Sxc.Render.StaticAssets.Sys;
using ToSic.Sxc.Ui.Context.Sys;
using ToSic.Sxc.WebApi.Sys.ExternalLinks;
using ToSic.Sys.Context;

namespace ToSic.Sxc.Run.Startup;

[InternalApi_DoNotUse_MayChangeWithoutNotice]
public static class StartupSxcWebApi
{
    public static IServiceCollection AddSxcWebApi(this IServiceCollection services)
    {
        // WIP - objects which are not really final
        services.TryAddTransient<ExternalLinksService>();

        // These are usually replaced by the target platform
        services.TryAddTransient<IAssetsExtractor, AssetsExtractorUnknown>();
            
        // Real Controllers

        // Backends
        services.TryAddTransient<IAppDetailsService, AppDetailsServiceExtended>(); // v22 - replace underlying service with Sxc version
        services.TryAddTransient<ExtensionExportService>();
        services.TryAddTransient<ExtensionReaderBackend>();
        services.TryAddTransient<ExtensionWriterBackend>();
        services.TryAddTransient<ExtensionInstallBackend>();
        services.TryAddTransient<ExtensionDownloadBackend>();
        // commented out, already registered in Eav.Apps.Persistence
        //services.TryAddTransient<ExtensionManifestService>();
        services.TryAddTransient<ExtensionInspectBackend>();
        services.TryAddTransient<ExtensionDeleteBackend>();
        //services.TryAddTransient<EntityBackend>();

        services.TryAddTransient<EditLoadBackend>();
        services.TryAddTransient<EditLoadActivityAddContentTypes>();
        services.TryAddTransient<EditLoadActivityAddNecessaryInputTypes>();
        services.TryAddTransient<EditLoadActivityAddContext>();
        services.TryAddTransient<EditLoadActivityAddRequiredFeatures>();
        services.TryAddTransient<EditLoadActivityAddPrefetch>();
        services.TryAddTransient<EditLoadActivitySettingsHelper>();
        services.TryAddTransient<EditLoadActivityCleanupRequest>();
        services.TryAddTransient<EditLoadActivityConvertRequest>();


        services.TryAddTransient<EditSaveBackend>();
        services.TryAddTransient<SaveSecurity>();
        services.TryAddTransient<AppViewPickerBackend>();
        services.TryAddTransient<ContentBlockBackend>();
        services.TryAddTransient<UsageBackend>();

        // Internal API helpers
        services.TryAddTransient<AppContent>();
        services.TryAddTransient<SxcPagePublishing>();

       // App import services moved to EAV WebApi in v22.
        services.TryAddTransient<ImportContent>();
        services.TryAddTransient<ExportContent>();
        services.TryAddTransient<ViewsExportImport>();

        // Small WebApi Helpers
        services.TryAddTransient<ContentGroupList>();

        // js context / UI
        services.TryAddTransient<SxcUiContextPartsService>();  // v22 - default implementation, for re-implementations
        services.TryAddTransient<IUiContextPartsService, SxcUiContextPartsService>();
        
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
        services.TryAddTransient<WorkQueryDeleteIfUnused>();
        services.TryAddTransient<CacheControllerReal>();
        services.TryAddTransient<EditControllerReal>();
        services.TryAddTransient<HistoryControllerReal>();
        services.TryAddTransient<ListControllerReal>();
        services.TryAddTransient<InstallControllerReal>();
        services.TryAddTransient<BlockControllerReal>();
        services.TryAddTransient<CodeControllerReal>();
        services.TryAddTransient<DataControllerReal>();

        services.TryAddTransient<AssetTemplates>();

        // Odata v21
        services.TryAddTransient<AppQueryODataHelper>();

        // SaveData PreSave
        services.TryAddTransient<DataValidatorContentTypeDataStore>();

        // Separate list Activities into isolated classes v22
        services.TryAddTransient<ListActivityReplaceOptions>();
        services.TryAddTransient<ListActivityReplace>();
        services.TryAddTransient<ListActivityGetItems>();
        services.TryAddTransient<ListActivitySave>();
        services.TryAddTransient<ListActivityGetBlockHeader>();

        services.TryAddTransient<IContextBuilderService, ContextBuilderService>();

        services.TryAddTransient<PermissionCheckAction>(); // new v22
        services.TryAddTransient<PublishWebApi>();  // new v22
        services.TryAddTransient<WorkViewsGetDetailed>();

        // v22 moved here from Sxc.Blocks
        services.AddContextProviders();

        // v22 moved here from Sxc.Web The top version should be deprecated soon, so we just use DataToDictionary or an Interface instead
        //services.TryAddTransient<IConvertToEavLight, ConvertToEavLightWithCmsInfo>();
        // New v22 - making it an add-on processor
        services.AddTransient<IConvertEntityToEavLightExtraProcessor, ConvertToEavLightWithCmsInfo>();

        services.TryAddTransient<InstallSettingsSvc>();

        services.AddLoadSettingsProviders();

        services.AddSxcInsights();

        services.AddFallbacks();

        return services;
    }

    /// <summary>
    /// This will add keyed data-source providers.
    /// Some may be replaced by platform implementations
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    /// <remarks>
    /// All calls in here MUST use TryAddTransient, and never without the Try
    /// </remarks>
    public static IServiceCollection AddSxcWorkDataSourceProviders(this IServiceCollection services)
    {
        // Original in case it needs to inherited
        services.TryAddTransient<SxcAppStatisticsProvider>();
        // for augmenting with view data
        services.TryAddKeyedTransient<IRawDataProviderWithCtx, SxcAppStatisticsProvider>(AppStatisticsProvider.NameId);

        return services;
    }



    public static IServiceCollection AddContextProviders(this IServiceCollection services)
    {
        // v22.01 2dm 2026-10-20 - only one service left, as we're trying to clean state sharing...
        services.TryAddScoped<ISxcCurrentContextService, SxcCurrentContextService>();
        services.TryAddScoped<IContextService>(x => x.GetRequiredService<ISxcCurrentContextService>());
        services.TryAddScoped<IContextOfSiteService>(x => x.GetRequiredService<ISxcCurrentContextService>());
        services.TryAddScoped<IContextCompleteService>(x => x.GetRequiredService<ISxcCurrentContextService>());

        return services;
    }

    public static IServiceCollection AddLoadSettingsProviders(this IServiceCollection services)
    {
        services.AddTransient<ILoadSettingsProvider, LoadSettingsForGpsDefaults>();
        services.AddTransient<ILoadSettingsProvider, LoadSettingsForContentType>();
        services.AddTransient<ILoadSettingsProvider, LoadSettingsApiKeys>();
        services.AddTransient<ILoadSettingsProvider, LoadSettingsForWysiwygDefaults>(); // new v21
        services.AddTransient<ILoadSettingsContentTypesProvider, LoadSettingsForPickerSources>();
        return services;
    }

    public static IServiceCollection AddAdamWebApi<TFolder, TFile>(this IServiceCollection services)
    {
        // Adam Controllers etc.
        services.TryAddTransient(typeof(HyperlinkBackend));
        services.TryAddTransient(typeof(AdamItemDtoMaker<,>));

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
