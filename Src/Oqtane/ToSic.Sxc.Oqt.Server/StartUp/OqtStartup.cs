using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Oqtane.Infrastructure;
using ToSic.Eav;
using ToSic.Eav.Configuration;
using ToSic.Eav.Plumbing;
using ToSic.Razor.StartUp;
using ToSic.Sxc.Oqt.Server.Adam.Imageflow;
using ToSic.Sxc.Oqt.Server.Controllers.AppApi;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Razor;
using ToSic.Sxc.WebApi;
using WebApiConstants = ToSic.Sxc.Oqt.Shared.WebApiConstants;

namespace ToSic.Sxc.Oqt.Server.StartUp
{
    public class OqtStartup : IServerStartup
    {
        public IConfiguration Configuration { get; }
        //public IWebHostEnvironment HostEnvironment { get; set; }

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
                .AddSxcRazor()                  // this is the .net core Razor compiler
                .AddAdamWebApi<int, int>()      // This is used to enable ADAM WebAPIs
                .AddSxcWebApi<IActionResult>()  // This adds all the standard backend services for WebAPIs to work
                .AddSxcCore()                   // Core 2sxc services
                .AddEav()                       // Core EAV services
                .AddOqtAppWebApi()              // Oqtane App WebAPI stuff
                .AddRazorBlade();               // RazorBlade helpers for Razor in the edition used by Oqtane

            // 2sxc Oqtane blob services for Imageflow.
            services.AddImageflowOqtaneBlobService();

            // Help RazorBlade to have a proper best-practices ToJson
            // New v12.05
            // 2022-02-01 2dm - should not be necessary any more, if we use .net 5.x DLLs (it is necessary if the .net standard 2 are used)
            // But we'll leave it in, because possibly this function is more reliable than the built in
            ToSic.Razor.StartUp.StartUp.RegisterToJson(JsonConvert.SerializeObject);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //HostEnvironment = env;

            var serviceProvider = app.ApplicationServices;

            serviceProvider.Build<IDbConfiguration>().ConnectionString = Configuration.GetConnectionString("DefaultConnection");

            var globalConfig = serviceProvider.Build<IGlobalConfiguration>();
            globalConfig.GlobalFolder = Path.Combine(env.ContentRootPath, "wwwroot\\Modules\\ToSic.Sxc");
            globalConfig.AssetsVirtualUrl = "~/Modules/ToSic.Sxc/assets/";
            globalConfig.SharedAppsFolder = $"/{OqtConstants.AppRoot}/{OqtConstants.SharedAppFolder}/"; // "/2sxc/Shared"


            // Load features from configuration
            // NOTE: On first installation of 2sxc module in oqtane, this code can not load all 2sxc global types
            // because it has dependency on ToSic_Eav_* sql tables, before this tables are actually created by oqtane 2.3.x,
            // but after next restart of oqtane application all is ok, and all 2sxc global types are loaded as expected
            var sysLoader = serviceProvider.Build<SystemLoader>();
            sysLoader.StartUp();

            app.UseExceptionHandler("/error");

            // routing middleware
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // endpoint mapping
            app.UseEndpoints(endpoints =>
            {
                // Release routes
                endpoints.Map(WebApiConstants.AppRoot + "/{appFolder}/api/{controller}/{action}", AppApiMiddleware.InvokeAsync);
                endpoints.Map(WebApiConstants.AppRoot + "/{appFolder}/{edition}/api/{controller}/{action}", AppApiMiddleware.InvokeAsync);
                endpoints.Map(WebApiConstants.AppRoot2 + "/{appFolder}/api/{controller}/{action}", AppApiMiddleware.InvokeAsync);
                endpoints.Map(WebApiConstants.AppRoot2 + "/{appFolder}/{edition}/api/{controller}/{action}", AppApiMiddleware.InvokeAsync);
                endpoints.Map(WebApiConstants.AppRoot3 + "/{appFolder}/api/{controller}/{action}", AppApiMiddleware.InvokeAsync);
                endpoints.Map(WebApiConstants.AppRoot3 + "/{appFolder}/{edition}/api/{controller}/{action}", AppApiMiddleware.InvokeAsync);

                // Beta routes
                endpoints.Map(WebApiConstants.WebApiStateRoot + "/app/{appFolder}/api/{controller}/{action}", AppApiMiddleware.InvokeAsync);
                endpoints.Map(WebApiConstants.WebApiStateRoot + "/app/{appFolder}/{edition}/api/{controller}/{action}", AppApiMiddleware.InvokeAsync);

                // Fallback route for 2sxc UI
                endpoints.MapFallbackToFile("/Modules/ToSic.Sxc/dist/ng-edit/", "/Modules/ToSic.Sxc/dist/ng-edit/index.html");
            });
        }

        public void ConfigureMvc(IMvcBuilder mvcBuilder)
        {
            // Do nothing
        }
    }
}
