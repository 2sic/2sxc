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
using ToSic.Sxc.WebApi.Features;
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
            services.TryAddTransient<Eav.Conversion.EntitiesToDictionary, DataToDictionary>();
            services.TryAddScoped<ILinkPaths, LinkPaths>();
            services.TryAddTransient<IServerPaths, ServerPaths>();
            services.TryAddTransient<XmlImportWithFiles, XmlImportFull>();
            services.TryAddTransient<TemplateHelpers, TemplateHelpers>();
            services.TryAddTransient<EngineBaseDependencies>();

            // These are usually replaced by the target platform
            services.TryAddTransient<IClientDependencyOptimizer, BasicClientDependencyOptimizer>();
            
            // Adam
            services.TryAddTransient(typeof(AdamAppContext<,>));
            services.TryAddTransient(typeof(AdamState<,>));
            services.TryAddTransient(typeof(HyperlinkBackend<,>));
            services.TryAddTransient(typeof(AdamTransGetItems<,>));
            services.TryAddTransient(typeof(AdamTransDelete<,>));
            services.TryAddTransient(typeof(AdamTransFolder<,>));
            services.TryAddTransient(typeof(AdamTransUpload<,>));
            services.TryAddTransient(typeof(AdamTransRename<,>));

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
            services.TryAddTransient<SaveSecurity>();

            // Helpers
            services.TryAddTransient<ImpExpHelpers>();


            // 11.08 - fallback in case not added
            services.TryAddSingleton<Run.Context.PlatformContext>();

            return services;
        }
    }
}
