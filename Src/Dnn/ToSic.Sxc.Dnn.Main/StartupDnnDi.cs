using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Caching;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.LookUp;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.Eav.Repositories;
using ToSic.Eav.Run;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps.Assets;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Adam;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.ImportExport;
using ToSic.Sxc.Dnn.Install;
using ToSic.Sxc.Dnn.LookUp;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Services;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.Admin;
using ToSic.Sxc.Dnn.WebApi.Context;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;
using ToSic.Sxc.WebApi.ApiExplorer;
using ToSic.Sxc.WebApi.Context;
using ToSic.Sxc.WebApi.Plumbing;


namespace ToSic.SexyContent
{

    internal static class StartupDnnDi
    {
        public static IServiceCollection AddDnn(this IServiceCollection services, string appsCacheOverride)
        {
            // Core Runtime Context Objects
            services.TryAddScoped<IUser, DnnUser>();
            services.TryAddScoped<ISite, DnnSite>();
            services.TryAddScoped<IZoneCultureResolver, DnnSite>();
            services.TryAddTransient<IModule, DnnModule>();
            services.TryAddTransient<DnnModule>();

            //
            services.TryAddTransient<IValueConverter, DnnValueConverter>();

            services.TryAddTransient<XmlExporter, DnnXmlExporter>();
            services.TryAddTransient<IImportExportEnvironment, DnnImportExportEnvironment>();

            // new for .net standard
            services.TryAddTransient<IAppFileSystemLoader, DnnAppFileSystemLoader>();
            services.TryAddTransient<IAppRepositoryLoader, DnnAppFileSystemLoader>();
            services.TryAddTransient<IZoneMapper, DnnZoneMapper>();

            services.TryAddTransient<IClientDependencyOptimizer, DnnClientDependencyOptimizer>();
            services.TryAddTransient<AppPermissionCheck, DnnPermissionCheck>();
            services.TryAddTransient<DnnPermissionCheck>();

            services.TryAddTransient<IDnnContext, DnnContextOld>();
            services.TryAddTransient<ILinkHelper, DnnLinkHelper>();
            services.TryAddTransient<DynamicCodeRoot, DnnDynamicCodeRoot>();
            services.TryAddTransient<DnnDynamicCodeRoot>();
            services.TryAddTransient<IPlatformModuleUpdater, DnnModuleUpdater>();
            services.TryAddTransient<IEnvironmentInstaller, DnnInstallationController>();

            // ADAM
            services.TryAddTransient<IAdamFileSystem<int, int>, DnnAdamFileSystem>();
            services.TryAddTransient<AdamManager, AdamManager<int, int>>();

            // Settings / WebApi stuff
            services.TryAddTransient<IUiContextBuilder, DnnUiContextBuilder>();
            services.TryAddTransient<IApiInspector, DnnApiInspector>();
            services.TryAddScoped<ResponseMaker, DnnResponseMaker>(); // must be scoped, as the api-controller must init this for use in other parts

            // new #2160
            services.TryAddTransient<AdamSecurityChecksBase, DnnAdamSecurityChecks>();

            services.TryAddTransient<ILookUpEngineResolver, DnnLookUpEngineResolver>();
            services.TryAddTransient<DnnLookUpEngineResolver>();
            services.TryAddTransient<IFingerprint, DnnFingerprint>();

            // new in 11.07 - exception logger
            services.TryAddTransient<IEnvironmentLogger, DnnEnvironmentLogger>();

            // new in 11.08 - provide Razor Engine and platform
            services.TryAddTransient<IRazorEngine, RazorEngine>();
            //services.TryAddTransient<RazorEngine>();
            services.TryAddSingleton<IPlatform, DnnPlatformContext>();

            // add page publishing
            services.TryAddTransient<IPagePublishing, Sxc.Dnn.Cms.DnnPagePublishing>();
            services.TryAddTransient<IPagePublishingResolver, Sxc.Dnn.Cms.DnnPagePublishingResolver>();

            // Asset Templates
            services.TryAddTransient<IAssetTemplates, DnnAssetTemplates>();

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
            
            // new in v12 - .net specific code compiler
            services.TryAddTransient<CodeCompiler, CodeCompilerNetFull>();

            // new in v12 - different way to integrate KOI - experimental!
            try
            {
                services.ActivateKoi2Di();
            } catch { /* ignore */ }
            
            // new in v12.02 - RazorBlade DI
            //services.TryAddScoped<IPageChangeApplicator, DnnPageChanges>();
            services.TryAddScoped<DnnPageChanges>();
            services.TryAddTransient<DnnClientResources>();

            return services;
        }
    }
}