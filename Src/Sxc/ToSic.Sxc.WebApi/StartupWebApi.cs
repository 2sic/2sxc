using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Run;
using ToSic.Eav.WebApi;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps.ImportExport;
using ToSic.Sxc.Conversion;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.Basic;
using ToSic.Sxc.Web.WebApi.System;
using ToSic.Sxc.WebApi.Adam;
using ToSic.Sxc.WebApi.App;
using ToSic.Sxc.WebApi.Cms;
using ToSic.Sxc.WebApi.ContentBlocks;
using ToSic.Sxc.WebApi.Context;
using ToSic.Sxc.WebApi.Features;
using ToSic.Sxc.WebApi.ImportExport;
using ToSic.Sxc.WebApi.InPage;
using ToSic.Sxc.WebApi.Languages;
using ToSic.Sxc.WebApi.Save;
using ToSic.Sxc.WebApi.Usage;

namespace ToSic.Sxc.WebApi
{
    public static class StartupWebApi
    {
        public static IServiceCollection AddSxcWebApi(this IServiceCollection services)
        {
            services.TryAddTransient<Eav.Conversion.EntitiesToDictionary, DataToDictionary>();
            services.TryAddScoped<ILinkPaths, LinkPaths>();
            services.TryAddTransient<IServerPaths, ServerPaths>();
            services.TryAddTransient<XmlImportWithFiles, XmlImportFull>();
            services.TryAddTransient<TemplateHelpers, TemplateHelpers>();
            services.TryAddTransient<EngineBaseDependencies>();

            // These are usually replaced by the target platform
            services.TryAddTransient<IClientDependencyOptimizer, BasicClientDependencyOptimizer>();
            
            // Backends
            services.TryAddTransient<AppsBackend>();
            services.TryAddTransient<EntityPickerBackend>();
            services.TryAddTransient<EntityBackend>();
            services.TryAddTransient<EditLoadBackend>();
            services.TryAddTransient<EditSaveBackend>();
            services.TryAddTransient<AppViewPickerBackend>();
            services.TryAddTransient<ContentBlockBackend>();
            services.TryAddTransient<FeaturesBackend>();
            services.TryAddTransient<UsageBackend>();
            services.TryAddTransient<LanguagesBackend>();

            // APIs
            services.TryAddTransient<EntityPickerApi>();
            services.TryAddTransient<ContentTypeApi>();
            services.TryAddTransient<QueryApi>();
            services.TryAddTransient<ContentExportApi>();
            services.TryAddTransient<ContentImportApi>();

            // Internal API helpers
            services.TryAddTransient<EntityApi>();
            services.TryAddTransient<Insights>();
            services.TryAddTransient<AppContent>();
            services.TryAddTransient<SxcPagePublishing>();
            services.TryAddTransient<ExportApp>();
            services.TryAddTransient<ImportApp>();
            services.TryAddTransient<ImportContent>();
            services.TryAddTransient<ResetApp>();

            // Small WebApi Helpers
            services.TryAddTransient<IdentifierHelper>();
            services.TryAddTransient<ContentGroupList>();

            // js context / UI
            services.TryAddTransient<IUiContextBuilder, UiContextBuilderUnknown>();
            services.TryAddTransient<UiContextBuilderBase.Dependencies>();


            // Helpers
            services.TryAddTransient<ImpExpHelpers>();

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
