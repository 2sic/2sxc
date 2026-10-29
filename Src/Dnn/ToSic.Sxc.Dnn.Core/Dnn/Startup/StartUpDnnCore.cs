using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneCulture;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Eav.Data.Sys.ValueConverter;
using ToSic.Eav.ImportExport.Integration;
using ToSic.Eav.ImportExport.Sys.XmlExport;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Sxc.Adam.Sys.FileSystem;
using ToSic.Sxc.Apps.Sys.Installation;
using ToSic.Sxc.Blocks.Sys.BlockBuilder;
using ToSic.Sxc.Cms.Publishing.Sys;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Code.Sys.SourceCode;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Adam;
using ToSic.Sxc.Dnn.Cms;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Compile;
using ToSic.Sxc.Dnn.Compile.Sys;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Dnn.Features;
using ToSic.Sxc.Dnn.ImportExport;
using ToSic.Sxc.Dnn.Install;
using ToSic.Sxc.Dnn.LookUp;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Search;
using ToSic.Sxc.Dnn.Services;
using ToSic.Sxc.Dnn.StartUp;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Integration.Modules;
using ToSic.Sxc.Render.JsContext.Sys;
using ToSic.Sxc.Render.StaticAssets.Sys;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sxc.Sys.Integration.Installation;
using ToSic.Sxc.Web.Sys.Http;
using ToSic.Sys.Boot;
using ToSic.Sys.Capabilities.Platform;
using ToSic.Sys.Security.Permissions;
using ToSic.Sys.Users;
using ExecutionContext = ToSic.Sxc.Sys.ExecutionContext.ExecutionContext;

namespace ToSic.Sxc.Dnn.Startup;

internal static class StartUpDnnCore
{
    public static IServiceCollection AddDnnCore(this IServiceCollection services)
    {
        // Core Runtime Context Objects
        services.TryAddTransient<DnnUserFactory>();
        services.TryAddScoped<IUser>(sp => sp.GetRequiredService<DnnUserFactory>().GetUser());
        services.TryAddScoped<DnnSecurity>();
        // Make sure that ISite and IZoneCultureResolver use the same singleton!
        services.TryAddTransient<DnnCultureResolver>();
        services.TryAddScoped<DnnSiteFactory>(); // WIP v22 - separate factory job from the ISite itself - TODO: later should become transient!
        services.TryAddScoped<ISite>(sp => sp.GetRequiredService<DnnSiteFactory>().TempSite);    // this must come before the IZoneCultureResolver!
        services.TryAddScoped<IZoneCultureResolver>(x => x.GetRequiredService<DnnSiteFactory>().TempSite);

        // Module cannot yet be scoped, until we have a per-module scope at some time
        //services.TryAddTransient<IModule, DnnModule>();
        services.TryAddTransient<DnnModuleFactory>(); // new v22 TODO: MUST RENAME TO DnnModuleFactory
        services.TryAddTransient<IModuleFactory, DnnModuleFactory>(); // new v22

        // The page is transient, which seems wrong, but it has very little initialization and is deterministic, so this is fine ATM
        services.TryAddTransient<DnnPageFactory>(); // new v22
        services.TryAddTransient<IPage>(sp => sp.GetRequiredService<DnnPageFactory>().Create()); // new v22
        //services.TryAddTransient<IPage, DnnPage>(); // old

        services.TryAddTransient<IValueConverter, DnnValueConverter>();
        services.TryAddTransient<IXmlExporterIntegration, DnnXmlExporterIntegration>();  // new v22
        services.TryAddTransient<IImportExportEnvironment, DnnImportExportEnvironment>();

        services.TryAddTransient<IZoneMapper, DnnZoneMapper>();

        services.TryAddTransient<IAssetsExtractor, DnnAssetsExtractor>();
        services.TryAddTransient<IEnvironmentPermission, DnnEnvironmentPermission>();

        services.TryAddTransient<IDnnContext, DnnContext>();
        services.TryAddTransient<ILinkService, DnnLinkService>();
        services.TryAddTransient<IPlatformModuleUpdater, DnnModuleUpdater>();
        services.TryAddTransient<IEnvironmentInstaller, DnnEnvironmentInstaller>();
        services.TryAddTransient<IPlatformAppInstaller, DnnPlatformAppInstaller>();
        services.TryAddTransient<DnnEnvironmentInstaller>(); // Dnn Only
        services.TryAddTransient<DnnInstallLogger>();


        services.TryAddTransient<ExecutionContext, DnnExecutionContext>();
        services.TryAddTransient<DnnExecutionContext>();
        // New v14
        services.TryAddTransient(typeof(ExecutionContext<,>), typeof(DnnExecutionContext<,>));

        // ADAM
        services.TryAddTransient<IAdamFileSystem, DnnAdamFileSystem>();

        services.TryAddTransient<ILookUpEngineResolver, DnnLookUpEngineResolver>();
        services.TryAddTransient<DnnLookUpEngineResolver>();
        services.TryAddTransient<IPlatformInfo, DnnPlatformContext>();

        // new in 11.07 - exception logger
        services.TryAddTransient<IEnvironmentLogger, DnnEnvironmentLogger>();

        // new in 11.08 - provide Razor Engine and platform
        services.TryAddSingleton<IPlatform, DnnPlatformContext>();

        // add page publishing
        services.TryAddTransient<IPagePublishing, DnnPagePublishing>();

        // v13 option to not use page publishing... #SwitchServicePagePublishingResolver #2749
        services.AddTransient<IWorkBlockPublishingLookup, DnnWorkPublishingLookup>();

        // new in v12 - .net specific code compiler
        services.TryAddTransient<CodeCompiler, CodeCompilerNetFull>();
        services.TryAddTransient<IClassCompiler, CodeCompilerNetFull>();

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

        // v13.12
        services.AddTransient<IBootProcess, BootRegisterDnnFeatures>();   // must be Add, not TryAdd

        // v14
#pragma warning disable CS0618 // Type or member is obsolete
        services.TryAddTransient<IDynamicCodeService, DnnDynamicCodeService>();
#pragma warning restore CS0618 // Type or member is obsolete
        services.TryAddTransient<DnnDynamicCodeService.MyScopedServices>();   // new v15
        services.TryAddTransient<IRenderService, DnnRenderService>();

        // v15 - move ready check turbo into a service
        services.TryAddTransient<DnnReadyCheckTurbo>();
        //services.TryAddScoped<CodeRootFactory, DnnCodeRootFactory>();

        // v17 - Assembly compilation and caching infrastructure
        services.TryAddSingleton<IHostingEnvironmentWrapper, HostingEnvironmentWrapper>();
        services.TryAddTransient<IReferencedAssembliesProvider, ReferencedAssembliesProvider>();
        services.TryAddTransient<AppCodeCompiler, AppCodeCompilerNetFull>();
        
        //v17.01
        services.TryAddTransient<DnnRequirements>();

        // v20.03 - Shared assembly disk cache infrastructure (for both Razor and AppCode)
        services.TryAddTransient<AssemblyDiskCache>();

        return services;
    }
}