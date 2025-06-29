﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneCulture;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Eav.Data.ValueConverter.Sys;
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
using ToSic.Sxc.Dnn.Compile.Internal;
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
using ToSic.Sxc.Render.Sys.JsContext;
using ToSic.Sxc.Render.Sys.Output;
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
        services.AddTransient<IPagePublishingGetSettings, DnnPagePublishingGetSettings>();

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
        services.AddTransient<IBootProcess, DnnBootFeaturesRegistration>();   // must be Add, not TryAdd

        // v14
        services.TryAddTransient<IDynamicCodeService, DnnDynamicCodeService>();
        services.TryAddTransient<DnnDynamicCodeService.MyScopedServices>();   // new v15
        services.TryAddTransient<IRenderService, DnnRenderService>();

        // #RemoveBlocksIRenderService
//#pragma warning disable CS0618
//        services.TryAddTransient<Blocks.IRenderService, DnnRenderService>();  // Obsolete, but keep for the few apps we already released in v12
//#pragma warning restore CS0618

        // v15 - move ready check turbo into a service
        services.TryAddTransient<DnnReadyCheckTurbo>();
        //services.TryAddScoped<CodeRootFactory, DnnCodeRootFactory>();

        // v17
        services.TryAddSingleton<IHostingEnvironmentWrapper, HostingEnvironmentWrapper>();
        services.TryAddTransient<IReferencedAssembliesProvider, ReferencedAssembliesProvider>();
        services.TryAddTransient<AppCodeCompiler, AppCodeCompilerNetFull>();

        //v17.01
        services.TryAddTransient<DnnRequirements>();

        return services;
    }
}