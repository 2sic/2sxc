using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps.Internal.Insights;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.ImportExport.Internal;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.ImportExport;
using ToSic.Eav.WebApi.Sys.Insights;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Internal.ImportExport;
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
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Data.Internal.Convert;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Integration.Paths;

namespace ToSic.Sxc.Backend;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public static class StartupWebApi
{
    public static IServiceCollection AddSxcWebApi(this IServiceCollection services)
    {
        // The top version should be deprecated soon, so we just use DataToDictionary or an Interface instead
        services.TryAddTransient<ConvertToEavLight, ConvertToEavLightWithCmsInfo>(); // this is needed for all the EAV uses of conversion
        services.TryAddTransient<ConvertToEavLightWithCmsInfo>(); // WIP, not public, should use interface instead
        services.TryAddTransient<IConvertToEavLight, ConvertToEavLightWithCmsInfo>();

        services.TryAddScoped<ILinkPaths, LinkPaths>();
        services.TryAddTransient<XmlImportWithFiles, XmlImportFull>();
        services.TryAddTransient<EngineBase.MyServices>();
        services.TryAddTransient<EngineCheckTemplate>();
        services.TryAddTransient<EnginePolymorphism>();
        services.TryAddTransient<EngineAppRequirements>();

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
        services.TryAddTransient<AppFolder>();
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
        services.TryAddTransient<Eav.WebApi.Admin.IAppExplorerControllerDependency, AppFilesControllerReal>();
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
        services.TryAddTransient(typeof(AdamTransactionBase<,,>.MyServices));
        services.TryAddTransient<AdamSecurityChecksBase.MyServices>();
        services.TryAddTransient(typeof(AdamManager<,>));
        services.TryAddTransient(typeof(AdamContext<,>));
        services.TryAddTransient<AdamContext.MyServices>();
        services.TryAddTransient(typeof(HyperlinkBackend<,>));
        services.TryAddTransient(typeof(AdamTransGetItems<,>));
        services.TryAddTransient(typeof(AdamTransDelete<,>));
        services.TryAddTransient(typeof(AdamTransFolder<,>));
        services.TryAddTransient(typeof(AdamTransUpload<,>));
        services.TryAddTransient(typeof(AdamTransRename<,>));
        services.TryAddTransient(typeof(AdamItemDtoMaker<,>));
        services.TryAddTransient(typeof(AdamItemDtoMaker<,>.MyServices));

        // Storage
        services.TryAddTransient(typeof(AdamStorageOfSite<,>));
        services.TryAddTransient(typeof(AdamStorageOfField<,>));

        // Typed Adam
        services.TryAddTransient<IAdamTransGetItems, AdamTransGetItems<TFolder, TFile>>();

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