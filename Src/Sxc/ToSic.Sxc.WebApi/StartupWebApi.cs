using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.WebApi.Adam;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.ImportExport;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps.ImportExport;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Data;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;
using ToSic.Sxc.WebApi.Adam;
using ToSic.Sxc.WebApi.Admin;
using ToSic.Sxc.WebApi.Admin.Query;
using ToSic.Sxc.WebApi.App;
using ToSic.Sxc.WebApi.AppStack;
using ToSic.Sxc.WebApi.Cms;
using ToSic.Sxc.WebApi.ContentBlocks;
using ToSic.Sxc.WebApi.Context;
using ToSic.Sxc.WebApi.ImportExport;
using ToSic.Sxc.WebApi.InPage;
using ToSic.Sxc.WebApi.Save;
using ToSic.Sxc.WebApi.Usage;

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
            services.TryAddTransient<EngineBaseDependencies>();

            // These are usually replaced by the target platform
            services.TryAddTransient<IBlockResourceExtractor, BlockResourceExtractorUnknown>();
            
            // Real Controllers

            // Backends
            services.TryAddTransient<AppsBackend>();
            services.TryAddTransient<EntityPickerBackend>();
            services.TryAddTransient<EntityBackend>();
            services.TryAddTransient<EditLoadBackend>();
            services.TryAddTransient<EditSaveBackend>();
            services.TryAddTransient<AppViewPickerBackend>();
            services.TryAddTransient<ContentBlockBackend>();
            //services.TryAddTransient<FeaturesBackend>();
            //services.TryAddTransient<LicenseBackend>();
            services.TryAddTransient<UsageBackend>();
            //services.TryAddTransient<LanguagesBackend>();
            services.TryAddTransient<QueryControllerReal>();

            //// APIs
            //services.TryAddTransient<ApiExplorerBackend<THttpResponseType>>();

            // Internal API helpers
            //services.TryAddTransient<Insights>();
            services.TryAddTransient<AppContent>();
            services.TryAddTransient<SxcPagePublishing>();
            services.TryAddTransient<ExportApp>();
            services.TryAddTransient<ImportApp>();
            services.TryAddTransient<ImportContent>();
            services.TryAddTransient<ResetApp>();
            services.TryAddTransient<AppStackBackend>();


            // Small WebApi Helpers
            services.TryAddTransient<IdentifierHelper>();
            services.TryAddTransient<ContentGroupList>();

            // js context / UI
            services.TryAddTransient<IUiContextBuilder, UiContextBuilderUnknown>();
            services.TryAddTransient<UiContextBuilderBase.Dependencies>();
            
            // Helpers
            services.TryAddTransient<ImpExpHelpers>();

            // Adam shared code across the APIs
            services.TryAddTransient<AdamCode>();

            // new v13
            //services.TryAddTransient<ZoneBackend>();

            // New v13 - try to reduce Dnn/Oqtane code to the max, by creating ControllerReal objects which do everything
            services.TryAddTransient<DialogControllerReal>();
            services.TryAddTransient(typeof(AppControllerReal<>));
            services.TryAddTransient<EditControllerReal>();
            services.TryAddTransient<HistoryControllerReal>();
            services.TryAddTransient<ContentGroupControllerReal>();
            services.TryAddTransient(typeof(AdamControllerReal<>));
            services.TryAddTransient<ListControllerReal>();
            services.TryAddTransient<AppContentControllerReal>();
            services.TryAddTransient<ViewControllerReal>();
            services.TryAddTransient<AppPartsControllerReal>();

            return services;
        }

        public static IServiceCollection AddAdamWebApi<TFolder, TFile>(this IServiceCollection services)
        {
            // Adam Controllers etc.
            services.TryAddTransient(typeof(AdamManager<,>));
            services.TryAddTransient(typeof(AdamContext<,>));
            services.TryAddTransient(typeof(HyperlinkBackend<,>));
            services.TryAddTransient(typeof(AdamTransGetItems<,>));
            services.TryAddTransient(typeof(AdamTransDelete<,>));
            services.TryAddTransient(typeof(AdamTransFolder<,>));
            services.TryAddTransient(typeof(AdamTransUpload<,>));
            services.TryAddTransient(typeof(AdamTransRename<,>));
            services.TryAddTransient(typeof(AdamItemDtoMaker<,>));
            services.TryAddTransient(typeof(AdamItemDtoMaker<,>.Dependencies));

            // Typed Adam
            services.TryAddTransient<IAdamTransGetItems, AdamTransGetItems<TFolder, TFile>>();

            return services;
        }
    }
}
