using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.ImportExport;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.ImportExport;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Data;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;
using ToSic.Sxc.WebApi.Adam;
using ToSic.Sxc.WebApi.Admin;
using ToSic.Sxc.WebApi.Admin.AppFiles;
using ToSic.Sxc.WebApi.Admin.Query;
using ToSic.Sxc.WebApi.App;
using ToSic.Sxc.WebApi.AppStack;
using ToSic.Sxc.WebApi.Cms;
using ToSic.Sxc.WebApi.ContentBlocks;
using ToSic.Sxc.WebApi.Context;
using ToSic.Sxc.WebApi.ImportExport;
using ToSic.Sxc.WebApi.InPage;
using ToSic.Sxc.WebApi.Save;
using ToSic.Sxc.WebApi.Sys;
using ToSic.Sxc.WebApi.Usage;
using ToSic.Sxc.WebApi.Views;

namespace ToSic.Sxc.WebApi
{
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

            // These are usually replaced by the target platform
            services.TryAddTransient<IBlockResourceExtractor, BlockResourceExtractorUnknown>();
            
            // Real Controllers

            // Backends
            services.TryAddTransient<AppsBackend>();
            services.TryAddTransient<EntityPickerBackend>();
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
            services.TryAddTransient(typeof(ExportContent<>));
            services.TryAddTransient<ResetApp>();
            services.TryAddTransient<AppStackBackend>();
            services.TryAddTransient<AppFolder>();
            services.TryAddTransient(typeof(ViewsExportImport<>));

            // Small WebApi Helpers
            // #UnusedFeatureHistoryOfGroup 2022-07-05 2dm removed - probably clean up ca. Q4 2022
            // services.TryAddTransient<IdentifierHelper>();
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
            services.TryAddTransient<QueryControllerReal>();
            services.TryAddTransient(typeof(AppControllerReal<>));
            services.TryAddTransient(typeof(AppPartsControllerReal<>));
            services.TryAddTransient<DialogControllerReal>();
            services.TryAddTransient(typeof(TypeControllerReal<>));
            services.TryAddTransient(typeof(ViewControllerReal<>));
            services.TryAddTransient<AppDataControllerReal>();
            services.TryAddTransient<AppQueryControllerReal>();
            services.TryAddTransient<ContentGroupControllerReal>();
            services.TryAddTransient<EditControllerReal>();
            services.TryAddTransient<HistoryControllerReal>();
            services.TryAddTransient<ListControllerReal>();
            services.TryAddTransient(typeof(InstallControllerReal<>));
            services.TryAddTransient<BlockControllerReal>();
            services.TryAddTransient<CodeControllerReal>();

            services.AddLoadSettingsProviders();

            return services;
        }

        public static IServiceCollection AddLoadSettingsProviders(this IServiceCollection services)
        {
            services.AddTransient<ILoadSettingsProvider, LoadSettingsForGpsDefaults>();
            services.AddTransient<ILoadSettingsProvider, LoadSettingsForContentType>();
            services.AddTransient<ILoadSettingsProvider, LoadSettingsApiKeys>();
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
    }
}
