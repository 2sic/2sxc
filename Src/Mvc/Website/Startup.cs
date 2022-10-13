using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using RazorPartialToString.Services;
using ToSic.Eav.Serialization;
using ToSic.Sxc.Mvc;

namespace Website
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            HostEnvironment = environment;
        }


        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add razor pages dynamic compilation WIP
            services.AddRazorPages()
                // experiment
                // https://github.com/aspnet/samples/blob/master/samples/aspnetcore/mvc/runtimecompilation/MyApp/Startup.cs#L26
                .AddRazorRuntimeCompilation(options =>
                {
                    var configuredPath = Configuration["SxcRoot"];
                    var libraryPath = Path.GetFullPath(Path.Combine(HostEnvironment.ContentRootPath, configuredPath));
                    options.FileProviders.Add(new PhysicalFileProvider(libraryPath));
                });

            // enable use of HttpContext
            services.AddHttpContextAccessor();

            // enable webapi - include all controllers in the Sxc.Mvc assembly
            services.AddControllers(options => { options.AllowEmptyInputInBodyModelBinding = true; })
                // This is needed to preserve compatibility with previous api usage
                // Set the JSON serializer options
                .AddJsonOptions(options => options.JsonSerializerOptions.SetUnsafeJsonSerializerOptions())
                .PartManager.ApplicationParts.Add(new AssemblyPart(typeof(SxcMvc).Assembly));

            // enable use of UrlHelper for AbsolutePath
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();
            services.AddScoped(it => it.GetService<IUrlHelperFactory>()
                .GetUrlHelper(it.GetService<IActionContextAccessor>().ActionContext));

            // Try to get partial to string rendering
            services.AddTransient<IRazorPartialToStringRenderer, RazorPartialToStringRenderer>();
            //services.AddTransient<IRenderRazor, RenderRazor>();

            StartupEavAndSxc.ConfigureIoC(services, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseRouteDebugger();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            var defFiles = new DefaultFilesOptions();
            defFiles.DefaultFileNames.Clear();
            defFiles.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(defFiles);
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
