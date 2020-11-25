using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Caching;
using ToSic.Eav.LookUp;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.Eav.Repositories;
using ToSic.Eav.Run;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Cms.Publishing;
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
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;
using ToSic.Sxc.WebApi.Adam;
using ToSic.Sxc.WebForms.Web;

namespace ToSic.SexyContent
{

    internal static class StartupDnnDi
    {
        public static IServiceCollection AddDnn(this IServiceCollection services, string appsCacheOverride)
        {
            // WebForms implementations
            services.TryAddScoped<IHttp, WebFormsHttp>();
            services.TryAddScoped<WebFormsHttp>();

            // Core Runtime Context Objects
            services.AddScoped<IUser, DnnUser>();
            services.AddScoped<ISite, DnnSite>();
            services.AddTransient<IContainer, DnnContainer>();
            services.AddTransient<DnnContainer>();

            // 
            services.AddTransient<IValueConverter, DnnValueConverter>();

            services.AddTransient<XmlExporter, DnnXmlExporter>();
            services.AddTransient<IImportExportEnvironment, DnnImportExportEnvironment>();

            // new for .net standard
            services.AddTransient<IAppFileSystemLoader, DnnAppFileSystemLoader>();
            services.AddTransient<IAppRepositoryLoader, DnnAppFileSystemLoader>();
            services.AddTransient<IGetDefaultLanguage, DnnSite>();
            services.AddTransient<IZoneMapper, DnnZoneMapper>();

            services.AddTransient<IClientDependencyOptimizer, DnnClientDependencyOptimizer>();
            services.AddTransient<AppPermissionCheck, DnnPermissionCheck>();

            services.AddTransient<DynamicCodeRoot, DnnDynamicCodeRoot>();
            services.AddTransient<IPlatformModuleUpdater, DnnModuleUpdater>();
            services.AddTransient<IEnvironmentInstaller, InstallationController>();

            // ADAM 
            services.AddTransient<IAdamFileSystem<int, int>, DnnAdamFileSystem>();
            services.AddTransient<AdamAppContext, AdamAppContext<int, int>>();

            // new #2160
            services.AddTransient<SecurityChecksBase, DnnAdamSecurityChecks>();

            services.AddTransient<IGetEngine, GetDnnEngine>();
            services.AddTransient<GetDnnEngine>();
            services.AddTransient<IFingerprint, DnnFingerprint>();

            // new in 11.07 - exception logger
            services.AddTransient<IEnvironmentLogger, DnnEnvironmentLogger>();

            // new in 11.08 - provide Razor Engine and platform
            services.AddTransient<IEngineFinder, DnnEngineFinder>();
            services.AddSingleton<Sxc.Run.Context.PlatformContext, DnnPlatformContext>();

            // add page publishing
            services.AddTransient<IPagePublishing, Sxc.Dnn.Cms.DnnPagePublishing>();
            services.AddTransient<IPagePublishingResolver, Sxc.Dnn.Cms.DnnPagePublishingResolver>();

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