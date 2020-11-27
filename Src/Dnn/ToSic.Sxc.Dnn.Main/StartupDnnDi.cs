using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Apps.Run;
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
using ToSic.Sxc.Run.Context;
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
            services.TryAddScoped<IUser, DnnUser>();
            services.TryAddScoped<ISite, DnnSite>();
            services.TryAddTransient<IContainer, DnnContainer>();
            services.TryAddTransient<DnnContainer>();

            // 
            services.TryAddTransient<IValueConverter, DnnValueConverter>();

            services.TryAddTransient<XmlExporter, DnnXmlExporter>();
            services.TryAddTransient<IImportExportEnvironment, DnnImportExportEnvironment>();

            // new for .net standard
            services.TryAddTransient<IAppFileSystemLoader, DnnAppFileSystemLoader>();
            services.TryAddTransient<IAppRepositoryLoader, DnnAppFileSystemLoader>();
            services.TryAddTransient<IGetDefaultLanguage, DnnSite>();
            services.TryAddTransient<IZoneMapper, DnnZoneMapper>();

            services.TryAddTransient<IClientDependencyOptimizer, DnnClientDependencyOptimizer>();
            services.TryAddTransient<AppPermissionCheck, DnnPermissionCheck>();
            services.TryAddTransient<DnnPermissionCheck>();

            services.TryAddTransient<DynamicCodeRoot, DnnDynamicCodeRoot>();
            services.TryAddTransient<DnnDynamicCodeRoot>();
            services.TryAddTransient<IPlatformModuleUpdater, DnnModuleUpdater>();
            services.TryAddTransient<IEnvironmentInstaller, InstallationController>();

            // ADAM 
            services.TryAddTransient<IAdamFileSystem<int, int>, DnnAdamFileSystem>();
            services.TryAddTransient<AdamAppContext, AdamAppContext<int, int>>();

            // new #2160
            services.TryAddTransient<SecurityChecksBase, DnnAdamSecurityChecks>();

            services.TryAddTransient<IGetEngine, GetDnnEngine>();
            services.TryAddTransient<GetDnnEngine>();
            services.TryAddTransient<IFingerprint, DnnFingerprint>();

            // new in 11.07 - exception logger
            services.TryAddTransient<IEnvironmentLogger, DnnEnvironmentLogger>();

            // new in 11.08 - provide Razor Engine and platform
            services.TryAddTransient<IEngineFinder, DnnEngineFinder>();
            services.TryAddSingleton<Sxc.Run.Context.PlatformContext, DnnPlatformContext>();

            // add page publishing
            services.TryAddTransient<IPagePublishing, Sxc.Dnn.Cms.DnnPagePublishing>();
            services.TryAddTransient<IPagePublishingResolver, Sxc.Dnn.Cms.DnnPagePublishingResolver>();

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