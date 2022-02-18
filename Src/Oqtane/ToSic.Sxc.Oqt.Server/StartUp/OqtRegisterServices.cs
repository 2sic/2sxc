using Microsoft.AspNetCore.Mvc;
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
using ToSic.Eav.Security;
using ToSic.Eav.WebApi.ApiExplorer;
using ToSic.Eav.WebApi.Context;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Client.Services;
using ToSic.Sxc.Oqt.Server.Adam;
using ToSic.Sxc.Oqt.Server.Apps;
using ToSic.Sxc.Oqt.Server.Blocks;
using ToSic.Sxc.Oqt.Server.Blocks.Output;
using ToSic.Sxc.Oqt.Server.Cms;
using ToSic.Sxc.Oqt.Server.Code;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Data;
using ToSic.Sxc.Oqt.Server.Installation;
using ToSic.Sxc.Oqt.Server.Integration;
using ToSic.Sxc.Oqt.Server.LookUps;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Oqt.Server.Services;
using ToSic.Sxc.Oqt.Server.WebApi;
using ToSic.Sxc.Oqt.Server.WebApi.Admin;
using ToSic.Sxc.Run;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web;
using ToSic.Sxc.WebApi.Plumbing;
using OqtPageOutput = ToSic.Sxc.Oqt.Server.Blocks.Output.OqtPageOutput;

namespace ToSic.Sxc.Oqt.Server.StartUp
{
    // ReSharper disable once InconsistentNaming
    internal static partial class OqtRegisterServices
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
            services.TryAddTransient<OqtGetBlock>();    // WIP - should replace most of OqtState
            services.TryAddScoped<RequestHelper>();

            services.TryAddTransient<IZoneCultureResolver, OqtSite>();
            services.TryAddTransient<IZoneMapper, OqtZoneMapper>();
            services.TryAddTransient<OqtZoneMapper>();
            services.TryAddTransient<AppPermissionCheck, OqtPermissionCheck>();
            services.TryAddTransient<IEnvironmentPermission, OqtEnvironmentPermission>();

            services.TryAddTransient<ILinkHelper, OqtLinkHelper>();
            services.TryAddTransient<DynamicCodeRoot, OqtaneDynamicCodeRoot>();
            services.TryAddTransient<IPlatformModuleUpdater, OqtModuleUpdater>();
            services.TryAddTransient<IEnvironmentInstaller, OqtEnvironmentInstaller>();
            services.TryAddTransient<ILookUpEngineResolver, OqtGetLookupEngine>();
            services.TryAddTransient<IPlatformInfo, OqtPlatformContext>();
            services.TryAddTransient<IUiContextBuilder, OqtUiContextBuilder>();
            services.TryAddTransient<OqtCulture>();
            services.TryAddTransient<SettingsHelper>();

            // add page publishing
            services.TryAddTransient<IPagePublishing, OqtPagePublishing>();
            services.TryAddTransient<IPagePublishingResolver, OqtPagePublishingResolver>();

            // Oqtane Specific stuff
            services.TryAddTransient<OqtPageOutput>();
            services.TryAddTransient<OqtSxcViewBuilder>();
            services.TryAddTransient<IBlockResourceExtractor, OqtBlockResourceExtractor>();
            services.TryAddTransient<IValueConverter, OqtValueConverter>();

            services.AddSingleton<IPlatform, OqtPlatformContext>();

            // ADAM stuff
            services.TryAddTransient<IAdamPaths, OqtAdamPaths>();
            services.TryAddTransient<IAdamFileSystem<int, int>, OqtAdamFileSystem>();
            services.TryAddTransient<AdamManager, AdamManager<int, int>>();

            // Still pending...
            services.TryAddTransient<XmlExporter, OqtXmlExporter>();
            services.TryAddTransient<IImportExportEnvironment, OqtImportExportEnvironment>();

            // View Builder
            services.TryAddTransient<OqtSxcViewBuilder>();

            // Site State Initializer for APIs etc. to ensure that the SiteState exists and is correctly preloaded
            services.TryAddTransient<SiteStateInitializer>();

            // Experimental - it seems that Blazor hides the url params in the request
            services.TryAddTransient<IHttp, HttpBlazor>();

            // Resolve appFolder when appName is "auto"
            services.TryAddTransient<OqtAppFolder>();
            services.TryAddTransient<AppAssetsDependencies>();

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
            services.TryAddScoped<ResponseMaker<IActionResult>, OqtResponseMaker>();

            // new in v12 - integrate KOI
            try
            {
                services.ActivateKoi2Di();
            }
            catch { /* ignore */ }

            services.TryAddTransient<OqtModuleHelper>();

            // v12.05
            services.TryAddTransient<ILogService, OqtLogService>();

            // v12.05
            services.TryAddTransient<IMailService, OqtMailService>();

            // v12.06
            services.TryAddSingleton<GlobalTypesCheck>();

            // ToSic.Sxc.Oqt.Client
            services.TryAddScoped<IPrerenderService, OqtPrerenderService>();

            // v13
            services.TryAddTransient<IModuleAndBlockBuilder, OqtModuleAndBlockBuilder>();

            return services;
        }



    }
}
