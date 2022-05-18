using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Configuration;
using System.Net.Http;
using ToSic.Eav;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Caching;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.LookUp;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.Eav.Repositories;
using ToSic.Eav.Run;
using ToSic.Eav.Security;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.ApiExplorer;
using ToSic.Eav.WebApi.Context;
using ToSic.Razor.StartUp;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Blocks;
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
using Type = System.Type;


namespace ToSic.Sxc.Dnn.StartUp
{
    public static class DnnDi
    {
        private static bool _alreadyRegistered;

        public static IServiceCollection RegisterServices(IServiceCollection services)
        {
            if (_alreadyRegistered)
                return OriginalServiceCollection;

            // If this is called from Dnn 7 - 9.3 it won't have services, so we must create our own
            // This is because the old Dnn wasn't DI aware
            if (services == null) services = new ServiceCollection();

            //var appsCache = GetAppsCacheOverride(); // 2022-05-18: commented because it not in use anymore
            services.AddDnn(/*appsCache*/)
                .AddAdamWebApi<int, int>()
                .AddSxcWebApi()
                .AddSxcCore()
                .AddEav()
                .AddEavWebApiTypedAfterEav<HttpResponseMessage>()
                .AddRazorBlade();

            // temp polymorphism - later put into AddPolymorphism
            services.TryAddTransient<Koi>();
            services.TryAddTransient<Permissions>();

            // Remember this for later, when we must start the Static Dependency Injection
            OriginalServiceCollection = services;

            _alreadyRegistered = true;
            return services;
        }

        public static Func<IServiceProvider> GetPreparedServiceProvider = null;

        public static IServiceCollection OriginalServiceCollection;

        // 2022-05-18: commented because it not in use anymore
        ///// <summary>
        ///// Expects something like "ToSic.Sxc.Dnn.DnnAppsCacheFarm, ToSic.Sxc.Dnn.Enterprise" - namespaces + class, DLL name without extension
        ///// </summary>
        ///// <returns></returns>
        //internal static string GetAppsCacheOverride()
        //{
        //    var farmCacheName = ConfigurationManager.AppSettings["EavAppsCache"];
        //    if (string.IsNullOrWhiteSpace(farmCacheName)) return null;
        //    return farmCacheName;
        //}


        public static IServiceCollection AddDnn(this IServiceCollection services /*, string appsCacheOverride*/)
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
            services.TryAddTransient<IEnvironmentPermission, DnnEnvironmentPermission>();

            services.TryAddTransient<IDnnContext, DnnContext>();
            services.TryAddTransient<ILinkService, DnnLinkService>();
            services.TryAddTransient<DynamicCodeRoot, DnnDynamicCodeRoot>();
            services.TryAddTransient<DnnDynamicCodeRoot>();
            services.TryAddTransient<IPlatformModuleUpdater, DnnModuleUpdater>();
            services.TryAddTransient<IEnvironmentInstaller, DnnEnvironmentInstaller>();
            services.TryAddTransient<DnnEnvironmentInstaller>(); // Dnn Only
            services.TryAddTransient<DnnInstallLogger>();

            // ADAM
            services.TryAddTransient<IAdamFileSystem<int, int>, DnnAdamFileSystem>();
            services.TryAddTransient<AdamManager, AdamManager<int, int>>();

            // Settings / WebApi stuff
            services.TryAddTransient<IUiContextBuilder, DnnUiContextBuilder>();
            services.TryAddTransient<IApiInspector, DnnApiInspector>();

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

            // v13 option to not use page publishing... #SwitchServicePagePublishingResolver #2749
            services.AddTransient<IPagePublishingSettings, Cms.DnnPagePublishingSettings>();

            // 2022-05-18: commented because it not in use anymore
            // new cache implements IAppsCacheSwitchable and it is registered with DNN DI.
            //if (appsCacheOverride != null)
            //{
            //    try
            //    {
            //        // replace default cache implementation with farm cache
            //        services.Remove(ServiceDescriptor.Singleton<IAppsCache, AppsCache>());
            //        var appsCacheType = Type.GetType(appsCacheOverride);
            //        services.TryAddSingleton(typeof(IAppsCache), appsCacheType);
            //    }
            //    catch
            //    {
            //        /* fallback */
            //        services.TryAddSingleton<IAppsCache, AppsCache>();
            //    }
            //}

            // new in v12 - .net specific code compiler
            services.TryAddTransient<CodeCompiler, CodeCompilerNetFull>();

            // Integrate KOI Dnn-Parts
            services.TryAddTransient<Connect.Koi.Detectors.ICssFrameworkDetector, Connect.Koi.Dnn.DetectAndCacheDnnThemeCssFramework>();
            
            // new in v12.02 - RazorBlade DI
            services.TryAddScoped<DnnPageChanges>();
            services.TryAddTransient<DnnClientResources>();

            // v12.04 - proper DI for SearchController
            services.TryAddTransient<SearchController>();

            // v12.05 custom Http for Dnn which only keeps the URL parameters really provided, and not the internally generated ones
            services.TryAddTransient<IHttp, DnnHttp>();

            // v12.05
            services.TryAddTransient<ILogService, DnnLogService>();
            services.TryAddTransient<IMailService, DnnMailService>();

            // v13
            services.TryAddTransient<IModuleAndBlockBuilder, DnnModuleAndBlockBuilder>();

            // v13.04
            services.TryAddTransient<IUserInformationService, DnnUserInformationService>();

            // v13.11/12
            services.AddTransient<IStartUpRegistrations, DnnStartUpRegistrations>();   // must be Add, not TryAdd

            return services;
        }
    }
}