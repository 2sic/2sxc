using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ToSic.Eav;
using ToSic.Eav.Configuration;
using ToSic.Eav.Plumbing;

namespace IntegrationSamples.BasicEav01
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _connStringFromConfig = configuration.GetConnectionString("SiteSqlServer");
        }

        private readonly string _connStringFromConfig;

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            // Enable EAV
            services.AddEav();

            // RazorPages - standard .net core MVC feature
            services.AddRazorPages();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // ----- Start EAV stuff -----
            var serviceProvider = app.ApplicationServices;
            var connectionString = _connStringFromConfig;
            serviceProvider.Build<IDbConfiguration>().ConnectionString = connectionString;
            var globalConfig = serviceProvider.Build<IGlobalConfiguration>();
            globalConfig.GlobalFolder = Path.Combine(env.ContentRootPath, "sys-2sxc");
            serviceProvider.Build<SystemLoader>().StartUp();
            // ----- Start EAV stuff -----


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
            });
        }
    }
}
