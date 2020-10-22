using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Oqtane.Infrastructure;
using ToSic.Eav;
using ToSic.Sxc.OqtaneModule.Shared.Dev;
using ToSic.Sxc.WebApi.Plumbing;
using Factory = ToSic.Eav.Factory;

namespace ToSic.Sxc.OqtaneModule.Server
{
    class Startup : IServerStartup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostEnvironment { get; }


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
            ToSic.Eav.Repository.Efc.Implementations.Configuration.SetConnectionString(connectionString);
            ToSic.Eav.Repository.Efc.Implementations.Configuration.SetFeaturesHelpLink("https://2sxc.org/help?tag=features", "https://2sxc.org/r/f/");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //// Add razor pages dynamic compilation WIP
            //services.AddRazorPages()
            //    // experiment
            //    // https://github.com/aspnet/samples/blob/master/samples/aspnetcore/mvc/runtimecompilation/MyApp/Startup.cs#L26
            //    .AddRazorRuntimeCompilation(options =>
            //    {
            //        var configuredPath = Configuration["SxcRoot"];
            //        var libraryPath = Path.GetFullPath(Path.Combine(HostEnvironment.ContentRootPath, configuredPath));
            //        options.FileProviders.Add(new PhysicalFileProvider(libraryPath));
            //    });

            // enable use of HttpContext
            services.AddHttpContextAccessor();

            // enable webapi - include all controllers in the Sxc.Mvc assembly
            services.AddControllers(options => { options.AllowEmptyInputInBodyModelBinding = true; })
                // This is needed to preserve compatibility with previous api usage
                .AddNewtonsoftJson(options =>
                {
                    // this ensures that c# objects with Pascal-case keep that
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    ToSic.Eav.ImportExport.Json.JsonSettings.Defaults(options.SerializerSettings);
                    //options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                });
            //.PartManager.ApplicationParts.Add(new AssemblyPart(typeof(SxcMvc).Assembly));

            // enable use of UrlHelper for AbsolutePath
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
            //services.AddScoped(it => it.GetService<IUrlHelperFactory>()
            //    .GetUrlHelper(it.GetService<IActionContextAccessor>().ActionContext));

            // Try to get partial to string rendering
            services.AddTransient<IRazorPartialToStringRenderer, RazorPartialToStringRenderer>();
            //services.AddTransient<IRenderRazor, RenderRazor>();

            Factory.UseExistingServices(services);
            Factory.ActivateNetCoreDi(services2 =>
            {
                services2
                    .AddSxcOqtane()
                    .AddSxc()
                    .AddEav();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //throw new NotImplementedException();
        }

        public void ConfigureMvc(IMvcBuilder mvcBuilder)
        {
            //throw new NotImplementedException();
        }
    }
}
