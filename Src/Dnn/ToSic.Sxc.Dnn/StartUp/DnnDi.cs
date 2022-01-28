using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Caching;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.LookUp;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.Eav.Repositories;
using ToSic.Eav.Run;
using ToSic.Eav.WebApi.ApiExplorer;
using ToSic.Eav.WebApi.Context;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Adam;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Dnn.ImportExport;
using ToSic.Sxc.Dnn.Install;
using ToSic.Sxc.Dnn.LookUp;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Services;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.Admin;
using ToSic.Sxc.Dnn.WebApi.Context;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Polymorphism;
using ToSic.Sxc.Run;
using ToSic.Sxc.Search;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Plumbing;
using Type = System.Type;


namespace ToSic.Sxc.Dnn.StartUp
{
    public static class DnnDi
    {
        private static bool _alreadyRegistered;

        public static void RegisterServices(IServiceCollection services)
        {
            if (_alreadyRegistered)
                return;

            // If this is called from Dnn 7 - 9.3 it won't have services, so we must create our own
            // This is because the old Dnn wasn't DI aware
            if (services == null) services = new ServiceCollection();

            var appsCache = GetAppsCacheOverride();
            services.AddDnn(appsCache)
                .AddAdamWebApi<int, int>()
                .AddSxcWebApi()
                .AddSxcCore()
                .AddEav();

            // temp polymorphism - later put into AddPolymorphism
            services.TryAddTransient<Koi>();
            services.TryAddTransient<Permissions>();

            // Remember this for later, when we must start the Static Dependency Injection
            DnnStaticDi.StaticServiceCollection = services;

            _alreadyRegistered = true;
        }

        /// <summary>
        /// Expects something like "ToSic.Sxc.Dnn.DnnAppsCacheFarm, ToSic.Sxc.Dnn.Enterprise" - namespaces + class, DLL name without extension
        /// </summary>
        /// <returns></returns>
        internal static string GetAppsCacheOverride()
        {
            var farmCacheName = ConfigurationManager.AppSettings["EavAppsCache"];
            if (string.IsNullOrWhiteSpace(farmCacheName)) return null;
            return farmCacheName;
        }


        public static IServiceCollection AddDnn(this IServiceCollection services, string appsCacheOverride)
        {
            // Core Runtime Context Objects
            services.TryAddScoped<IUser, DnnUser>();

            // Make sure that ISite and IZoneCultureResolver use the same singleton!
            services.TryAddScoped<ISite, DnnSite>();    // this must come first!
            services.TryAddScoped<IZoneCultureResolver>(x => x.GetRequiredService<ISite>());
            
            // Module cannot yet be scoped, until we have a per-module scope at some time
            services.TryAddTransient<IModule, DnnModule>();
            services.TryAddTransient<IValueConverter, DnnValueConverter>();

            services.TryAddTransient<XmlExporter, DnnXmlExporter>();
            services.TryAddTransient<IImportExportEnvironment, DnnImportExportEnvironment>();

            // new for .net standard
            // todo: find out why we have both - do they have a different function? probably yes....
            services.TryAddTransient<IAppFileSystemLoader, DnnAppFileSystemLoader>();
            services.TryAddTransient<IAppRepositoryLoader, DnnAppFileSystemLoader>();

            services.TryAddTransient<IZoneMapper, DnnZoneMapper>();

            services.TryAddTransient<IBlockResourceExtractor, DnnBlockResourceExtractor>();
            services.TryAddTransient<AppPermissionCheck, DnnPermissionCheck>();
            services.TryAddTransient<DnnPermissionCheck>();

            services.TryAddTransient<IDnnContext, DnnContext>();
            services.TryAddTransient<ILinkHelper, DnnLinkHelper>();
            services.TryAddTransient<DynamicCodeRoot, DnnDynamicCodeRoot>();
            services.TryAddTransient<DnnDynamicCodeRoot>();
            services.TryAddTransient<IPlatformModuleUpdater, DnnModuleUpdater>();
            services.TryAddTransient<IEnvironmentInstaller, DnnEnvironmentInstaller>();
            services.TryAddTransient<DnnInstallLogger>(sp =>
                ActivatorUtilities.CreateInstance<DnnInstallLogger>(sp, DnnEnvironmentInstaller.SaveUnimportantDetails));

            // ADAM
            services.TryAddTransient<IAdamFileSystem<int, int>, DnnAdamFileSystem>();
            services.TryAddTransient<AdamManager, AdamManager<int, int>>();

            // Settings / WebApi stuff
            services.TryAddTransient<IUiContextBuilder, DnnUiContextBuilder>();
            services.TryAddTransient<IApiInspector, DnnApiInspector>();
            services.TryAddScoped<ResponseMaker, DnnResponseMaker>(); // must be scoped, as the api-controller must init this for use in other parts

            // new #2160
            services.TryAddTransient<AdamSecurityChecksBase, DnnAdamSecurityChecks>();

            services.TryAddTransient<ILookUpEngineResolver, DnnLookUpEngineResolver>();
            services.TryAddTransient<DnnLookUpEngineResolver>();
            services.TryAddTransient<IPlatformInfo, DnnPlatformContext>();

            // new in 11.07 - exception logger
            services.TryAddTransient<IEnvironmentLogger, DnnEnvironmentLogger>();

            // new in 11.08 - provide Razor Engine and platform
            services.TryAddTransient<IRazorEngine, RazorEngine>();
            services.TryAddSingleton<IPlatform, DnnPlatformContext>();

            // add page publishing
            services.TryAddTransient<IPagePublishing, Cms.DnnPagePublishing>();
            services.TryAddTransient<IPagePublishingResolver, Cms.DnnPagePublishingResolver>();

            if (appsCacheOverride != null)
            {
                try
                {
                    var appsCacheType = Type.GetType(appsCacheOverride);
                    services.TryAddSingleton(typeof(IAppsCache), appsCacheType);
                }
                catch
                {
                    /* ignore */
                }
            }
            
            // new in v12 - .net specific code compiler
            services.TryAddTransient<CodeCompiler, CodeCompilerNetFull>();

            // new in v12 - different way to integrate KOI - experimental!
            try
            {
                services.ActivateKoi2Di();
            } catch { /* ignore */ }
            
            // new in v12.02 - RazorBlade DI
            services.TryAddScoped<DnnPageChanges>();
            services.TryAddTransient<DnnClientResources>();

            // v12.04 - proper DI for SearchController
            services.TryAddTransient<SearchController>();

            // v12.05 custom Http for Dnn which only keeps the URL parameters really provided, and not the internally generated ones
            services.TryAddTransient<IHttp, DnnHttp>();

            // v12.05
            services.TryAddTransient<ILogService, DnnLogService>();

            // v12.05
            services.TryAddTransient<IMailService, DnnMailService>();

            return services;
        }
    }
}