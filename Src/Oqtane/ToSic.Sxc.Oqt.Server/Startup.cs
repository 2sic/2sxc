using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Oqtane.Infrastructure;
using ToSic.Eav;
using ToSic.Sxc.Oqt.Server.RazorPages;
using ToSic.Sxc.Oqt.Shared.Dev;
using ToSic.Sxc.Razor.Engine;
using ToSic.Sxc.WebApi.Plumbing;
using Factory = ToSic.Eav.Factory;

namespace ToSic.Sxc.Oqt.Server
{
    class Startup : IServerStartup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostEnvironment { get; set; }


        public Startup()
        {
            var builder = new ConfigurationBuilder()
                //.SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();

            var devMode = Configuration["DevMode"];
            if (devMode == "SPM") TestIds.Dev4Spm = true;

            var connectionString = Configuration.GetConnectionString("SiteSqlServer");
            Eav.Repository.Efc.Implementations.Configuration.SetConnectionString(connectionString);
            Eav.Repository.Efc.Implementations.Configuration.SetFeaturesHelpLink("https://2sxc.org/help?tag=features", "https://2sxc.org/r/f/");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // try to enable dynamic razor compiling - still WIP
            new StartUpRazorPages().ConfigureServices(services);

            // enable webapi - include all controllers in the Sxc.Mvc assembly
            services.AddControllers(options => { options.AllowEmptyInputInBodyModelBinding = true; })
                // This is needed to preserve compatibility with previous api usage
                .AddNewtonsoftJson(options =>
                {
                    // this ensures that c# objects with Pascal-case keep that
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    Eav.ImportExport.Json.JsonSettings.Defaults(options.SerializerSettings);
                });
            //.PartManager.ApplicationParts.Add(new AssemblyPart(typeof(SxcMvc).Assembly));

            // enable use of UrlHelper for AbsolutePath
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();

            Factory.UseExistingServices(services);
            Factory.ActivateNetCoreDi(services2 =>
            {
                services2
                    .AddSxcOqtane()
                    .AddSxcRazor()
                    .AddSxcWebApi()
                    .AddSxcCore()
                    .AddEav();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            HostEnvironment = env;
            //_contentRootPath = env.ContentRootPath;
        }

        // Workaround because of initialization issues with razor pages
        //private static string _contentRootPath;

        public void ConfigureMvc(IMvcBuilder mvcBuilder)
        {
            //throw new NotImplementedException();
        }


    }
}
