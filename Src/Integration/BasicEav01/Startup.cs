using System.IO;
using IntegrationSamples.BasicEav01.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using ToSic.Eav;
using ToSic.Eav.Configuration;
using ToSic.Eav.Context;
using ToSic.Eav.Run;
using ToSic.Eav.StartUp;

namespace IntegrationSamples.BasicEav01
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
            // #2sxcIntegration
            // Register our Always-Super-User (to allow Insights to be used)
            services.TryAddTransient<IUser, IntUser>();
            // Enable all of EAV
            services.AddEav();

            // RazorPages - standard .net core MVC feature
            services.AddRazorPages();
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


            // Standard Stuff
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
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

                // #2sxcIntegration - enable insights controllers
                endpoints.MapControllers();
            });

        }
    }
}
