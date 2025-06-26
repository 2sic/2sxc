using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oqtane.Infrastructure;
using ToSic.Eav;
using ToSic.Eav.Integration;
using ToSic.Eav.Sys;
using ToSic.Lib.DI;
using ToSic.Razor.StartUp;
using ToSic.Sxc.Backend;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Images;
using ToSic.Sxc.Oqt.Server.Adam.Imageflow;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Controllers.AppApi;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Razor;
using ToSic.Sxc.Services;
using ToSic.Sxc.Startup;
using ToSic.Sys.Boot;
using ToSic.Sys.Configuration;
using ToSic.Sys.Security.Encryption;
using static ToSic.Sxc.Oqt.Server.WebApi.OqtWebApiConstants;

namespace ToSic.Sxc.Oqt.Server.StartUp;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class OqtStartup : IServerStartup
{
    internal IConfigurationRoot Configuration { get; }

    public OqtStartup()
    {
        // Configuration is used to provide Master tenant sql connection string to 2sxc eav.
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        Configuration = builder.Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // 1. Enable dynamic razor compiling
        services.AddRazorPages()
            .AddRazorRuntimeCompilation(options =>
            {
                var dllLocation = typeof(Oqtane.Server.Program).Assembly.Location;
                var dllPath = Path.GetDirectoryName(dllLocation);
                foreach (var dllFile in Directory.GetFiles(dllPath, "*.dll"))
                    options.AdditionalReferencePaths.Add(dllFile);
            });

        // TODO: STV - MAKE SURE OUR CONTROLLERS RULES ONLY APPLY TO OURS, NOT TO override rules on normal Oqtane controllers

        // 2. Register EAV & 2sxc
        services
            .AddSxcOqtane()                 // Always first add your override services
            .AddOqtSxcDataSources()
            .AddSxcRazor()                  // this is the .net core Razor compiler
            .AddAdamWebApi<int, int>()      // This is used to enable ADAM WebAPIs
            .AddSxcWebApi()                 // This adds all the standard backend services for WebAPIs to work
            .AddSxcCustom()                 // Anything around custom code
            .AddSxcCode()
            .AddSxcCodeHotBuild()
            .AddSxcEngines()
            .AddSxcImages()
            // Core 2sxc services
            .AddSxcApps()
            .AddSxcEdit()
            .AddSxcData()
            .AddSxcAdam()
            .AddSxcAdamWork<int, int>()
            .AddSxcBlocks()
            .AddSxcRender()
            .AddSxcCms()
            .AddSxcServices()
            .AddSxcServicesObsolete()
            .AddSxcWeb()
            .AddSxcLightSpeed()             // LightSpeed services
            .AddSxcCodeGen()                // Code generation services
            .AddSxcCoreNew()
            .AddSxcAppsFallbackServices()
            .AddSxcCoreFallbackServices()
            .AddEavEverything()             // Core EAV services
            .AddEavEverythingFallbacks()
            .AddEavWebApiTypedAfterEav()
            .AddOqtAppWebApi()              // Oqtane App WebAPI stuff
            .AddRazorBlade();               // RazorBlade helpers for Razor in the edition used by Oqtane

        // 2sxc Oqtane blob services for Imageflow and other customizations.
        services.AddImageflowExtensions();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var serviceProvider = app.ApplicationServices;

        var globalConfig = serviceProvider.Build<IGlobalConfiguration>();
        globalConfig.ConnectionString(Configuration.GetConnectionString("DefaultConnection"));
        globalConfig.GlobalFolder(Path.Combine(env.ContentRootPath, "wwwroot\\Modules", OqtConstants.PackageName));
        globalConfig.AppDataTemplateFolder(Path.Combine(env.ContentRootPath, "Content", "2sxc", "system", FolderConstants.AppDataProtectedFolder, FolderConstants.NewAppFolder));
        globalConfig.DataFolder(Path.Combine(env.ContentRootPath, "Content", "2sxc", "system", FolderConstants.AppDataProtectedFolder, FolderConstants.FolderSystem));
        globalConfig.TemporaryFolder(Path.Combine(env.ContentRootPath, "Content", "2sxc", "system", FolderConstants.TemporaryFolder));
        globalConfig.InstructionsFolder(Path.Combine(env.ContentRootPath, "Content", "2sxc", "system", FolderConstants.InstructionsFolder));
        globalConfig.AssetsVirtualUrl($"~/Modules/{OqtConstants.PackageName}/assets/");
        globalConfig.SharedAppsFolder($"/{OqtConstants.AppRoot}/{OqtConstants.SharedAppFolder}/"); // "/2sxc/Shared"
        globalConfig.TempAssemblyFolder(Path.Combine(env.ContentRootPath, FolderConstants.AppDataProtectedFolder, FolderConstants.TempAssemblyFolder)); // ".../App_Data/2sxc.bin"
        globalConfig.CryptoFolder(Path.Combine(env.ContentRootPath, FolderConstants.AppDataProtectedFolder, FolderConstants.CryptoFolder));

        // ensure we have an instance
        var assemblyResolver = serviceProvider.Build<AssemblyResolver>();

        // Load features from configuration
        // NOTE: On first installation of 2sxc module in oqtane, this code can not load all 2sxc global types
        // because it has dependency on ToSic_Eav_* sql tables, before this tables are actually created by oqtane 2.3.x,
        // but after next restart of oqtane application all is ok, and all 2sxc global types are loaded as expected

        var bootCoordinator = serviceProvider.Build<BootCoordinator>();
        bootCoordinator.StartUp();

        // Clean the App_Data/2sxc.bin folder
        serviceProvider.Build<Util>().CleanTempAssemblyFolder();

        if (env.IsDevelopment())
            app.UsePageResponseRewriteMiddleware();

        // Configure Sxc endpoints and dialogs
        app.UseEndpoints(endpoints =>
        {
            // Sxc API Endpoints
            // Mapped directly; these will be matched based on their patterns.
            foreach (var pattern in SxcEndpointPatterns)
                endpoints.Map(pattern, AppApiMiddleware.InvokeAsync);

            // Sxc Dialogs
            // Mapped as fallbacks for specific URL patterns.
            foreach (var (url, page, setting) in SxcDialogs)
                endpoints.MapFallback(url, context => EditUiMiddleware.PageOutputCached(context, env, page, setting));
        });
    }

    public void ConfigureMvc(IMvcBuilder mvcBuilder)
    {
        // Do nothing
    }
}