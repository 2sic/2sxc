using System;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Caching;
using ToSic.Eav.LookUp;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.Eav.Plumbing.Booting;
using ToSic.Eav.Repositories;
using ToSic.Eav.Run;
using ToSic.SexyContent.Dnn920;
using ToSic.Sxc;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Adam;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.ImportExport;
using ToSic.Sxc.Dnn.Install;
using ToSic.Sxc.Dnn.LookUp;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Web;
using ToSic.Sxc.Polymorphism;
using ToSic.Sxc.Run;
using ToSic.Sxc.WebApi.Adam;
using ToSic.Sxc.WebApi.Plumbing;
using Factory = ToSic.Eav.Factory;

namespace ToSic.SexyContent
{
    /// <inheritdoc />
    /// <summary>
    /// this configures unity (the IoC container)
    /// Never call this directly! always go through Settings.Ensure...
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class ConfigurationLoader: IConfigurationLoader
    {
        private static bool _alreadyConfigured;

        /// <summary>
        /// Configure IoC for 2sxc. If it's already configured, do nothing.
        /// </summary>
        public void Configure()
        {
            if (_alreadyConfigured)
                return;

            ConfigureConnectionString();
            var appsCache = GetAppsCacheOverride();
            Factory.ActivateNetCoreDi(services =>
            {
                services
                    .AddDnn(appsCache)
                    .AddSxcWebApi()
                    .AddSxcCore()
                    .AddEav();
            });
            SharpZipLibRedirect.RegisterSharpZipLibRedirect();
            ConfigurePolymorphResolvers();
            _alreadyConfigured = true;
        }


        private void ConfigureConnectionString()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString;
            Eav.Repository.Efc.Implementations.Configuration.SetConnectionString(connectionString);
            Eav.Repository.Efc.Implementations.Configuration.SetFeaturesHelpLink("https://2sxc.org/help?tag=features", "https://2sxc.org/r/f/");
        }

        /// <summary>
        /// Expects something like "ToSic.Sxc.Dnn.DnnAppsCacheFarm, ToSic.Sxc.Dnn.Enterprise" - namespaces + class, DLL name without extension
        /// </summary>
        /// <returns></returns>
        private string GetAppsCacheOverride()
        {
            var farmCacheName = ConfigurationManager.AppSettings["EavAppsCache"];
            if (string.IsNullOrWhiteSpace(farmCacheName)) return null;
            return farmCacheName;
        }

        /// <summary>
        /// Configure all the known polymorph resolvers
        /// </summary>
        private void ConfigurePolymorphResolvers()
        {
            Polymorphism.Add(new Koi());
            Polymorphism.Add(new Permissions());
        }
    }

    internal static class DnnDependencyInjection
    {
        public static IServiceCollection AddDnn(this IServiceCollection services, string appsCacheOverride)
        {
            services.AddTransient<IValueConverter, DnnValueConverter>();
            services.AddTransient<IUser, DnnUser>();

            services.AddTransient<XmlExporter, DnnXmlExporter>();
            services.AddTransient<IImportExportEnvironment, DnnImportExportEnvironment>();

            // new for .net standard
            services.AddScoped<ISite, DnnSite>();
            services.AddTransient<IContainer, DnnContainer>();
            services.AddTransient<IAppFileSystemLoader, DnnAppFileSystemLoader>();
            services.AddTransient<IAppRepositoryLoader, DnnAppFileSystemLoader>();
            services.AddScoped<IEnvironment, DnnEnvironment>();
            services.AddScoped<IAppEnvironment, DnnEnvironment>();
            services.AddTransient<IZoneMapper, DnnZoneMapper>();

            services.AddTransient<IClientDependencyOptimizer, DnnClientDependencyOptimizer>();
            services.AddTransient<AppPermissionCheck, DnnPermissionCheck>();

            services.AddTransient<DynamicCodeRoot, DnnDynamicCodeRoot>();
            services.AddTransient<IRenderingHelper, DnnRenderingHelper>();
            services.AddTransient<IPlatformModuleUpdater, DnnModuleUpdater>();
            services.AddTransient<IEnvironmentInstaller, InstallationController>();

            // ADAM 
            services.AddTransient<IAdamFileSystem<int, int>, DnnAdamFileSystem>();
            services.AddTransient<AdamAppContext, AdamAppContext<int, int>>();

            // new #2160
            services.AddTransient<SecurityChecksBase, DnnAdamSecurityChecks>();

            services.AddTransient<IGetEngine, GetDnnEngine>();
            services.AddTransient<IFingerprint, DnnFingerprint>();

            // new in 11.07 - exception logger
            services.AddTransient<IEnvironmentLogger, DnnEnvironmentLogger>();

            // new in 11.08 - provide Razor Engine and platform
            services.AddTransient<IEngineFinder, DnnEngineFinder>();
            services.AddSingleton<Sxc.Run.Context.PlatformContext, DnnPlatformContext>();

            // add page publishing
            services.AddTransient<IPagePublishing, Sxc.Dnn.Cms.DnnPagePublishing>();

            if (appsCacheOverride != null)
            {
                try
                {
                    var appsCacheType = Type.GetType(appsCacheOverride);
                    services.TryAddSingleton(typeof(IAppsCache), appsCacheType);
                }
                catch
                {
                    /* ignore */
                }
            }

            return services;
        }
    }
}