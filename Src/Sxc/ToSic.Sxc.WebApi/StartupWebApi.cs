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

namespace ToSic.Sxc.WebApi
{
    public static class StartupWebApi
    {
        public static IServiceCollection AddSxcWebApi(this IServiceCollection services)
        {
            services.TryAddTransient<Eav.Conversion.EntitiesToDictionary, DataToDictionary>();
            services.TryAddScoped<IHttp, HttpAbstraction>();
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

            // Internal API helpers
            services.TryAddTransient<EntityApi>();
            services.TryAddTransient<EntityPickerApi>();
            services.TryAddTransient<EntityPickerBackend>();
            services.TryAddTransient<Insights>();
            services.TryAddTransient<ContentTypeApi>();
            services.TryAddTransient<AppContent>();
            services.TryAddTransient<EditLoadBackend>();


            // 11.08 - fallback in case not added
            services.TryAddSingleton<Run.Context.PlatformContext>();

            return services;
        }
    }
}
