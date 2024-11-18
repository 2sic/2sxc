using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oqtane.Components;
using Oqtane.Extensions;
using Oqtane.Infrastructure;
using Oqtane.Shared;
using Oqtane.UI;
using OqtaneSSR.Extensions;
using System.IO;
using ToSic.Eav.Integration;
using ToSic.Eav.Internal.Configuration;
using ToSic.Eav.Internal.Loaders;
using ToSic.Eav.WebApi;
using ToSic.Lib.DI;
using ToSic.Razor.StartUp;
using ToSic.Sxc.Backend;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Integration.Startup;
using ToSic.Sxc.Oqt.Server.Adam.Imageflow;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Controllers.AppApi;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Razor;
using ToSic.Sxc.Startup;
using static ToSic.Sxc.Oqt.Server.StartUp.OqtStartupHelper;
using static ToSic.Sxc.Oqt.Server.WebApi.OqtWebApiConstants;

namespace ToSic.Sxc.Oqt.Server.StartUp;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
            .AddSxcCore()                   // Core 2sxc services
            .AddSxcCodeGen()                // Code generation services
            .AddEavEverything()             // Core EAV services
            .AddEavWebApiTypedAfterEav()
            .AddOqtAppWebApi()              // Oqtane App WebAPI stuff
            .AddRazorBlade();               // RazorBlade helpers for Razor in the edition used by Oqtane

        // 2sxc Oqtane blob services for Imageflow and other customizations.
        services.AddImageflowExtensions();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //#region Unhandled Errors (should be in the start off pipeline)
        //if (env.IsDevelopment())
        //{
        //    app.UseDeveloperExceptionPage();
        //}
        //else
        //{
        //    app.UseExceptionHandler("/error"); // This will redirect to the ErrorLocalDevelopment action in ErrorController when an exception occurs
        //}
        //#endregion

        var serviceProvider = app.ApplicationServices;

        serviceProvider.Build<IDbConfiguration>().ConnectionString = Configuration.GetConnectionString("DefaultConnection");

        var globalConfig = serviceProvider.Build<IGlobalConfiguration>();
        globalConfig.GlobalFolder = Path.Combine(env.ContentRootPath, "wwwroot\\Modules", OqtConstants.PackageName);
        globalConfig.AppDataTemplateFolder = Path.Combine(env.ContentRootPath, "Content", "2sxc", "system", Eav.Constants.AppDataProtectedFolder, Eav.Constants.NewAppFolder);
        globalConfig.NewAppsTemplateFolder = Path.Combine(env.ContentRootPath, "Content", "2sxc", "system", Eav.Constants.AppDataProtectedFolder, Eav.Constants.NewAppsFolder);
        globalConfig.DataFolder = Path.Combine(env.ContentRootPath, "Content", "2sxc", "system", Eav.Constants.AppDataProtectedFolder, Eav.Constants.FolderSystem);
        globalConfig.TemporaryFolder = Path.Combine(env.ContentRootPath, "Content", "2sxc", "system", Eav.Constants.TemporaryFolder);
        globalConfig.InstructionsFolder = Path.Combine(env.ContentRootPath, "Content", "2sxc", "system", Eav.Constants.InstructionsFolder);
        globalConfig.AssetsVirtualUrl = $"~/Modules/{OqtConstants.PackageName}/assets/";
        globalConfig.SharedAppsFolder = $"/{OqtConstants.AppRoot}/{OqtConstants.SharedAppFolder}/"; // "/2sxc/Shared"
        globalConfig.TempAssemblyFolder = Path.Combine(env.ContentRootPath, Eav.Constants.AppDataProtectedFolder, Eav.Constants.TempAssemblyFolder); // ".../App_Data/2sxc.bin"
        globalConfig.CryptoFolder = Path.Combine(env.ContentRootPath, Eav.Constants.AppDataProtectedFolder, Eav.Constants.CryptoFolder);

        // ensure we have an instance
        var assemblyResolver = serviceProvider.Build<AssemblyResolver>();

        // Load features from configuration
        // NOTE: On first installation of 2sxc module in oqtane, this code can not load all 2sxc global types
        // because it has dependency on ToSic_Eav_* sql tables, before this tables are actually created by oqtane 2.3.x,
        // but after next restart of oqtane application all is ok, and all 2sxc global types are loaded as expected

        var sxcSysLoader = serviceProvider.Build<SystemLoader>();
        sxcSysLoader.StartUp();

        // Clean the App_Data/2sxc.bin folder
        serviceProvider.Build<Util>().CleanTempAssemblyFolder();

        if (env.IsDevelopment())
            app.UsePageResponseRewriteMiddleware();

        // MapWhen split the middleware pipeline into two completely separate branches
        app.MapWhen(context => IsSxcEndpoint(context.Request.Path.Value), appBuilder =>
        {
            appBuilder.UseOqtaneMiddlewareConfiguration();
            appBuilder.UseEndpoints(endpoints =>
            {
                foreach (var pattern in SxcEndpointPatterns)
                    endpoints.Map(pattern, AppApiMiddleware.InvokeAsync);
            });
            // end of this middleware pipeline branch
        });

        app.MapWhen(context => IsSxcDialog(context.Request.Path.Value), appBuilder =>
        {
            appBuilder.UseOqtaneMiddlewareConfiguration();
            appBuilder.UseEndpoints(endpoints =>
            {
                // Handle / Process URLs to Dialogs route for 2sxc UI
                foreach (var (url, page, setting) in SxcDialogs)
                    endpoints.MapFallback(url, context => EditUiMiddleware.PageOutputCached(context, env, page, setting));
            });
        });
    }

    public void ConfigureMvc(IMvcBuilder mvcBuilder)
    {
        // Do nothing
    }
}

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseOqtaneMiddlewareConfiguration(this IApplicationBuilder app)
    {
        // Oqtane middlewares should be executed before configuration of 2sxc endpoint mappings
        // to avoid having duplicate middleware in pipeline (like we had before).
        // Order of middleware configuration is important, because that is order of middleware execution in pipeline.

        var serviceProvider = app.ApplicationServices;
        var corsService = serviceProvider.Build<ICorsService>();
        var corsPolicyProvider = serviceProvider.Build<ICorsPolicyProvider>();

        #region Oqtane copy from Startup.cs - L197

        // allow oqtane localization middleware
        app.UseOqtaneLocalization();

        app.UseHttpsRedirection();
        app.UseStaticFiles(new StaticFileOptions
        {
            ServeUnknownFileTypes = true,
            OnPrepareResponse = (ctx) =>
            {
                var policy = corsPolicyProvider.GetPolicyAsync(ctx.Context, Constants.MauiCorsPolicy)
                    .ConfigureAwait(false).GetAwaiter().GetResult();
                corsService.ApplyResult(corsService.EvaluatePolicy(ctx.Context, policy), ctx.Context.Response);
            }
        });
        app.UseExceptionMiddleWare();
        //app.UseTenantResolution(); // commented, because it breaks alias resolving in 2sxc and it will resolve siteid=1 for all sites
        app.UseJwtAuthorization();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();

        //if (_useSwagger)
        //{
        //  app.UseSwagger();
        //  app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/" + Constants.Version + "/swagger.json", Constants.PackageId + " " + Constants.Version); });
        //}

        app.UseEndpoints(endpoints =>
        {
          endpoints.MapControllers();
          endpoints.MapRazorPages();
        });

        app.UseEndpoints(endpoints =>
        {
          endpoints.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(SiteRouter).Assembly);
        });

        // simulate the fallback routing approach of traditional Blazor - allowing the custom SiteRouter to handle all routing concerns
        app.UseEndpoints(endpoints =>
        {
          endpoints.MapFallback();
        });

        #endregion
        return app;
    }
}