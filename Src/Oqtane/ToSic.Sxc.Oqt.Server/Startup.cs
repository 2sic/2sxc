using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Oqtane.Infrastructure;
using System.IO;
using ToSic.Eav;
using ToSic.Eav.Caching;
using ToSic.Eav.Configuration;
using ToSic.Eav.Persistence.File;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Oqt.Server.Adam.Imageflow;
using ToSic.Sxc.Oqt.Server.Controllers.AppApi;
using ToSic.Sxc.Oqt.Server.StartUp;
using ToSic.Sxc.Razor;
using ToSic.Sxc.WebApi;
using WebApiConstants = ToSic.Sxc.Oqt.Shared.WebApiConstants;

namespace ToSic.Sxc.Oqt.Server
{
    public class Startup : IServerStartup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostEnvironment { get; set; }

        public Startup()
        {
            // Configuration is used to provide Master tenant sql connection string to 2sxc eav.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // try to enable dynamic razor compiling - still WIP
            services.AddRazorPages()            
                .AddRazorRuntimeCompilation(options =>
                {
                    // Add razor pages dynamic compilation WIP
                    //var ContentRootPath = Path.GetFullPath(Path.Combine(HostEnvironment.ContentRootPath, OqtConstants.ContentSubfolder));
                    //options.FileProviders.Add(new PhysicalFileProvider(ContentRootPath));

                    var dllLocation = typeof(Oqtane.Server.Program).Assembly.Location;
                    var dllPath = Path.GetDirectoryName(dllLocation);
                    foreach (var dllFile in Directory.GetFiles(dllPath, "*.dll"))
                        options.AdditionalReferencePaths.Add(dllFile);
                });

            // TODO: STV - MAKE SURE OUR CONTROLLERS RULES ONLY APPLY TO OURS, NOT TO override rules on normal Oqtane controllers
            // enable webapi - include all controllers in the Sxc.Mvc assembly
            //services
            //    .AddControllers(options =>
            //    {
            //        // options.AllowEmptyInputInBodyModelBinding = true; // Added with attribute
            //        // options.Filters.Add(new HttpResponseExceptionFilter()); // Added with attribute
            //    });
            // This is needed to preserve compatibility with previous api usage
            //.AddNewtonsoftJson(options =>
            //{
            //    // this ensures that c# objects with Pascal-case keep that
            //    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            //    Eav.ImportExport.Json.JsonSettings.Defaults(options.SerializerSettings);
            //});

            services
                .AddSxcOqtane()
                .AddSxcRazor()
                .AddAdamWebApi<int, int>()
                .AddSxcWebApi()
                .AddSxcCore()
                .AddEav()
                .AddAppApi(); // 2sxc Oqtane dyncode app api.

            // 2sxc Oqtane blob services for Imageflow.
            services.AddImageflowOqtaneBlobService();

            // Help RazorBlade to have a proper best-practices ToJson
            // New v12.05
            ToSic.Razor.Internals.StartUp.RegisterToJson(JsonConvert.SerializeObject);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            HostEnvironment = env;

            var serviceProvider = app.ApplicationServices;

            serviceProvider.Build<IDbConfiguration>().ConnectionString = Configuration.GetConnectionString("DefaultConnection");

            var globalConfig = serviceProvider.Build<IGlobalConfiguration>();
            globalConfig.GlobalFolder = Path.Combine(HostEnvironment.ContentRootPath, "wwwroot\\Modules\\ToSic.Sxc");
            globalConfig.GlobalSiteFolder = "todo - global apps not implemented yet";


            // Load features from configuration
            // NOTE: On first installation of 2sxc module in oqtane, this code can not load all 2sxc global types
            // because it has dependency on ToSic_Eav_* sql tables, before this tables are actually created by oqtane 2.3.x,
            // but after next restart of oqtane application all is ok, and all 2sxc global types are loaded as expected
            var sysLoader = serviceProvider.Build<SystemLoader>();
            sysLoader.StartUp();

            // 2021-11-16 2dm - experimental, working on moving global/preset data into a normal AppState #PresetInAppState
            sysLoader.Log.Add("Try to load global app-state");
            var globalStateLoader = serviceProvider.Build<FileAppStateLoaderWIP>();
            var appState = globalStateLoader.AppState(Eav.Constants.PresetAppId);
            var appsMemCache = serviceProvider.Build<IAppsCache>();
            appsMemCache.Add(appState);
            // End experimental #PresetInAppState


            app.UseExceptionHandler("/error");
            //app.UseDeveloperExceptionPage();

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
