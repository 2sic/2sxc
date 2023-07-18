using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.LookUp;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.Eav.Repositories;
using ToSic.Eav.Run;
using ToSic.Eav.Security;
using ToSic.Eav.StartUp;
using ToSic.Eav.WebApi;
using ToSic.Razor.StartUp;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Compatibility;
using ToSic.Sxc.Context;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Dnn.Adam;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Dnn.ImportExport;
using ToSic.Sxc.Dnn.Install;
using ToSic.Sxc.Dnn.LookUp;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Services;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Polymorphism;
using ToSic.Sxc.Run;
using ToSic.Sxc.Search;
using ToSic.Sxc.Services;
using ToSic.Sxc.Startup;
using ToSic.Sxc.Web;
using ToSic.Sxc.WebApi;


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

            services
                .AddDnn()
                .AddDnnCore() // TODO: Move core stuff from AddDnn to AddDnnCore and make implementations internal
                .AddDnnSxcDataSources()
                //.AddDnnCore()
                .AddDnnDataSources()
                .AddDnnWebApi()
                .AddDnnRazor()
                .AddDnnCompatibility()
                .AddAdamWebApi<int, int>()
                .AddSxcWebApi()
                .AddSxcCore()
                .AddEav()
                .AddEavWebApiTypedAfterEav()
                .AddRazorBlade();

            // temp polymorphism - later put into AddPolymorphism
            services.TryAddTransient<Koi>();
            services.TryAddTransient<Permissions>();

            // Remember this for later, when we must start the Static Dependency Injection
            OriginalServiceCollection = services;

            _alreadyRegistered = true;
            return services;
        }

        public static IServiceCollection OriginalServiceCollection;

        public static IServiceCollection AddDnn(this IServiceCollection services)
        {
            // Core Runtime Context Objects
            services.TryAddScoped<IUser, DnnUser>();
            services.TryAddScoped<DnnSecurity>();
            // Make sure that ISite and IZoneCultureResolver use the same singleton!
            services.TryAddScoped<ISite, DnnSite>();    // this must come first!
            services.TryAddScoped<IZoneCultureResolver>(x => x.GetRequiredService<ISite>());
            
            // Module cannot yet be scoped, until we have a per-module scope at some time
            services.TryAddTransient<IModule, DnnModule>();
            services.TryAddTransient<IPage, DnnPage>();
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
            services.TryAddTransient<IPlatformModuleUpdater, DnnModuleUpdater>();
            services.TryAddTransient<IEnvironmentInstaller, DnnEnvironmentInstaller>();
            services.TryAddTransient<IPlatformAppInstaller, DnnPlatformAppInstaller>();
            services.TryAddTransient<DnnEnvironmentInstaller>(); // Dnn Only
            services.TryAddTransient<DnnInstallLogger>();


            services.TryAddTransient<DynamicCodeRoot, DnnDynamicCodeRoot>();
            services.TryAddTransient<DnnDynamicCodeRoot>();
            // New v14
            services.TryAddTransient(typeof(DynamicCodeRoot<,>), typeof(DnnDynamicCodeRoot<,>));
            //services.TryAddTransient(typeof(DnnDynamicCodeRoot<,>));
            //services.TryAddTransient<DnnCodeRootFactory>();
            //services.AddDnnCore();
            //services.TryAddTransient<CodeRootFactory, DnnCodeRootFactory>();



            // ADAM
            services.TryAddTransient<IAdamFileSystem<int, int>, DnnAdamFileSystem>();
            services.TryAddTransient<AdamManager, AdamManager<int, int>>();

            //// Settings / WebApi stuff
            //services.TryAddTransient<IUiContextBuilder, DnnUiContextBuilder>();
            //services.TryAddTransient<IApiInspector, DnnApiInspector>();

            //// new #2160
            //services.TryAddTransient<AdamSecurityChecksBase, DnnAdamSecurityChecks>();

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
            services.AddTransient<IPagePublishingGetSettings, Cms.DnnPagePublishingGetSettings>();

            // new in v12 - .net specific code compiler
            services.TryAddTransient<CodeCompiler, CodeCompilerNetFull>();

            // Integrate KOI Dnn-Parts
            services.TryAddTransient<Connect.Koi.Detectors.ICssFrameworkDetector, Connect.Koi.Dnn.DetectAndCacheDnnThemeCssFramework>();
            
            // new in v12.02 - RazorBlade DI
            services.TryAddScoped<DnnPageChanges>();
            services.TryAddTransient<DnnClientResources>();
            services.TryAddScoped<DnnJsApiHeader>(); // v16.01
            services.TryAddScoped<IJsApiService, DnnJsApiService>(); // v16.01

            // v12.04 - proper DI for SearchController
            services.TryAddTransient<SearchController>();

            // v12.05 custom Http for Dnn which only keeps the URL parameters really provided, and not the internally generated ones
            services.TryAddTransient<IHttp, DnnHttp>();

            // v12.05
            services.TryAddTransient<ISystemLogService, DnnSystemLogService>();
            services.TryAddTransient<IMailService, DnnMailService>();

            // v13
            services.TryAddTransient<IModuleAndBlockBuilder, DnnModuleAndBlockBuilder>();

            // v13.04
            services.TryAddTransient<IUsersService, DnnUsersService>();

            // v13.12
            services.AddTransient<IStartUpRegistrations, DnnStartUpRegistrations>();   // must be Add, not TryAdd

            // v14
            services.TryAddTransient<IDynamicCodeService, DnnDynamicCodeService>();
            services.TryAddTransient<DnnDynamicCodeService.MyScopedServices>();   // new v15
            services.TryAddTransient<Sxc.Services.IRenderService, DnnRenderService>();
#pragma warning disable CS0618
            services.TryAddTransient<Blocks.IRenderService, DnnRenderService>();  // Obsolete, but keep for the few apps we already released in v12
#pragma warning restore CS0618

            //services.TryAddTransient<DnnGetBlock>();
            services.TryAddTransient<DnnAppFolderUtilities>(); // v14.12-01

            // v15 - move ready check turbo into a service
            services.TryAddTransient<DnnReadyCheckTurbo>();

            return services;
        }
    }
}