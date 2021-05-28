using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oqtane.Infrastructure;
using System.IO;
using ToSic.Eav;
using ToSic.Eav.Configuration;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Oqt.Server.Adam.Imageflow;
using ToSic.Sxc.Oqt.Server.Controllers.AppApi;
using ToSic.Sxc.Oqt.Server.StartUp;
using ToSic.Sxc.Razor;
using ToSic.Sxc.WebApi;
using Factory = ToSic.Eav.Factory;
using WebApiConstants = ToSic.Sxc.Oqt.Shared.WebApiConstants;

namespace ToSic.Sxc.Oqt.Server
{
    class Startup : IServerStartup
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
            new StartUpRazorPages().ConfigureServices(services);

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


            Factory.UseExistingServices(services);
            Factory.ActivateNetCoreDi(services2 =>
            {
                services2
                    .AddSxcOqtane()
                    .AddSxcRazor()
                    .AddAdamWebApi<int, int>()
                    .AddSxcWebApi()
                    .AddSxcCore()
                    .AddEav()
                    .AddAppApi(); // 2sxc Oqtane dyncode app api.
            });

            var sp = services.BuildServiceProvider();

            sp.Build<IDbConfiguration>().ConnectionString = Configuration.GetConnectionString("DefaultConnection");

            var hostingEnvironment = sp.Build<IHostEnvironment>();
            var globalConfig = sp.Build<IGlobalConfiguration>();
            globalConfig.GlobalFolder = Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot\\Modules\\ToSic.Sxc");
            globalConfig.GlobalSiteFolder = "todo - global apps not implemented yet";

            // 2sxc Oqtane blob services for Imageflow.
            services.AddImageflowOqtaneBlobService();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            HostEnvironment = env;

            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/error-local-development");
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

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

        // Workaround because of initialization issues with razor pages
        //private static string _contentRootPath;

        public void ConfigureMvc(IMvcBuilder mvcBuilder)
        {
            //throw new NotImplementedException();
        }


    }
}
