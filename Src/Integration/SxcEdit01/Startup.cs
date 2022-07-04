using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ToSic.Eav;
using ToSic.Eav.Configuration;
using ToSic.Eav.Run;
using ToSic.Eav.StartUp;
using ToSic.Sxc.Startup;
using ToSic.Sxc.WebApi;

namespace IntegrationSamples.SxcEdit01
{
    public class Startup
    {
        /// <summary>
        /// This method gets called first by the runtime. Use it to get configuration values. 
        /// </summary>
        public Startup(IConfiguration configuration)
        {
            _connStringFromConfig = configuration.GetConnectionString("SiteSqlServer");
        }
        private readonly string _connStringFromConfig;

        /// <summary>
        /// This method gets called second by the runtime. Use this method to add services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            // #2sxcIntegration
            services
                .AddMvcRazor()
                .AddControllersAndConfigureJson()
                .AddImplementations()
                .AddAdamWebApi<string, string>()
                .AddSxcWebApi()
                .AddSxcCore()
                .AddEav();
        }


        /// <summary>
        /// This method gets called third by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // ----- Start EAV stuff #2sxcIntegration -----
            var serviceProvider = app.ApplicationServices;

            // Set Connection String
            serviceProvider.GetRequiredService<IDbConfiguration>().ConnectionString = _connStringFromConfig;

            // Set global path where it will find the .data folder
            var globalConfig = serviceProvider.GetRequiredService<IGlobalConfiguration>();
            globalConfig.GlobalFolder = Path.Combine(env.ContentRootPath, "sys-2sxc");

            // Trigger start where the data etc. will be loaded & initialized
            serviceProvider.GetRequiredService<SystemLoader>().StartUp();
            // ----- End EAV stuff #2sxcIntegration -----

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();

                // #2sxcIntegration - enable controllers
                endpoints.MapControllers();
            });
        }
    }
}
