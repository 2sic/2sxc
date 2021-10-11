using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.LookUp;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.Eav.Run;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Apps.Assets;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Adam;
using ToSic.Sxc.Oqt.Server.Block;
using ToSic.Sxc.Oqt.Server.Code;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.LookUps;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Server.WebApi;
using ToSic.Sxc.Oqt.Server.WebApi.Admin;
using ToSic.Sxc.Run;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web;
using ToSic.Sxc.WebApi.ApiExplorer;
using ToSic.Sxc.WebApi.Context;
using ToSic.Sxc.WebApi.Plumbing;

namespace ToSic.Sxc.Oqt.Server.StartUp
{
    // ReSharper disable once InconsistentNaming
    static partial class OqtaneDI
    {
        public static IServiceCollection AddSxcOqtane(this IServiceCollection services)
        {
            services.AddScoped<ILinkPaths, OqtLinkPaths>();
            services.TryAddTransient<IServerPaths, OqtServerPaths>();

            services.TryAddScoped<ISite, OqtSite>();
            services.TryAddScoped<IPage, OqtPage>();
            services.TryAddScoped<OqtSite>();
            services.TryAddScoped<IUser, OqtUser>();
            services.TryAddTransient<IModule, OqtModule>();
            services.TryAddTransient<OqtModule>();
            //services.TryAddScoped<OqtState>();
            services.TryAddTransient<OqtGetBlock>();    // WIP - should replace most of OqtState
            services.TryAddScoped<RequestHelper>();
            //services.TryAddTransient<OqtTempInstanceContext>();

            services.TryAddTransient<IZoneCultureResolver, OqtSite>();
            services.TryAddTransient<IZoneMapper, OqtZoneMapper>();
            services.TryAddTransient<OqtZoneMapper>();
            services.TryAddTransient<AppPermissionCheck, OqtPermissionCheck>();
            services.TryAddTransient<ILinkHelper, OqtLinkHelper>();
            services.TryAddTransient<DynamicCodeRoot, OqtaneDynamicCodeRoot>();
            services.TryAddTransient<IPlatformModuleUpdater, OqtModuleUpdater>();
            services.TryAddTransient<IEnvironmentInstaller, OqtEnvironmentInstaller>();
            services.TryAddTransient<ILookUpEngineResolver, OqtGetLookupEngine>();
            services.TryAddTransient<IFingerprint, OqtFingerprintWip>();
            services.TryAddTransient<IUiContextBuilder, OqtUiContextBuilder>();
            services.TryAddTransient<OqtCulture>();
            services.TryAddTransient<SettingsHelper>();

            //// add page publishing
            services.TryAddTransient<IPagePublishing, OqtPagePublishing>();
            services.TryAddTransient<IPagePublishingResolver, OqtPagePublishingResolver>();

            //// Oqtane Specific stuff
            services.TryAddTransient<OqtAssetsAndHeaders>();
            services.TryAddTransient<OqtSxcViewBuilder>();
            services.TryAddTransient<IClientDependencyOptimizer, OqtClientDependencyOptimizer>();
            services.TryAddTransient<IValueConverter, OqtValueConverter>();

            services.AddSingleton<IPlatform, OqtPlatformContext>();

            // ADAM stuff
            services.TryAddTransient<IAdamPaths, OqtAdamPaths>();
            services.TryAddTransient<IAdamFileSystem<int, int>, OqtAdamFileSystem>();
            services.TryAddTransient<AdamManager, AdamManager<int, int>>();
            //services.TryAddTransient(typeof(AdamItemDtoMaker<,>), typeof(OqtAdamItemDtoMaker<,>));

            //// Still pending...
            services.TryAddTransient<XmlExporter, OqtXmlExporter>();
            services.TryAddTransient<IImportExportEnvironment, OqtImportExportEnvironment>();
            //sc.TryAddTransient<IAppFileSystemLoader, DnnAppFileSystemLoader>();
            //sc.TryAddTransient<IAppRepositoryLoader, DnnAppFileSystemLoader>();

            // View Builder
            services.TryAddTransient<ISxcOqtane, OqtSxcViewBuilder>();

            // Site State Initializer for APIs etc. to ensure that the SiteState exists and is correctly preloaded
            services.TryAddTransient<SiteStateInitializer>();

            // Experimental - it seems that Blazor hides the url params in the request
            services.TryAddTransient<IHttp, HttpBlazor>();

            // Resolve appFolder when appName is "auto"
            services.TryAddTransient<OqtAppFolder>();
            services.TryAddTransient<AppAssetsDependencies>();

            // Asset Templates
            services.TryAddTransient<IAssetTemplates, OwtAssetTemplates>();

            // Lookup
            services.TryAddTransient<QueryStringLookUp>();
            services.TryAddTransient<SiteLookUp>();
            services.TryAddTransient<OqtPageLookUp>();
            services.TryAddTransient<OqtModuleLookUp>();
            services.TryAddScoped<UserLookUp>();

            // Polymorphism Resolvers
            services.TryAddTransient<Sxc.Polymorphism.Koi>();
            services.TryAddTransient<Polymorphism.Permissions>();

            // enable use of UrlHelper for AbsolutePath
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();

            // action filter for exceptions
            services.AddTransient<HttpResponseExceptionFilter>();

            // action filter instead of global option AllowEmptyInputInBodyModelBinding = true
            services.AddTransient<OptionalBodyFilter>();

            // new in v12 - .net specific code compiler
            services.TryAddTransient<CodeCompiler, CodeCompilerNetCore>();

            // APiExplorer
            services.TryAddTransient<IApiInspector, OqtApiInspector>();
            services.TryAddScoped<ResponseMaker, OqtResponseMaker>();

            // new in v12 - integrate KOI - experimental!
            try
            {
                services.ActivateKoi2Di();
            }
            catch { /* ignore */ }

            services.TryAddTransient<OqtModuleHelper>();

            // v12.05
            services.TryAddTransient<ILogService, OqtLogService>();

            return services;
        }



    }
}
