using System;
using System.Configuration;
using System.Web.Hosting;
using DotNetNuke.Web.Api;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using ToSic.Eav;
using ToSic.Eav.Caching;
using ToSic.Eav.Configuration;
using ToSic.Eav.Persistence.File;
using ToSic.SexyContent.Dnn920;
using ToSic.Sxc.Polymorphism;
using ToSic.Sxc.WebApi;
using GlobalConfiguration = System.Web.Http.GlobalConfiguration;

namespace ToSic.Sxc.Dnn.StartUp
{
    /// <summary>
    /// This configures .net Core Dependency Injection
    /// The StartUp is defined as an IServiceRouteMapper.
    /// This way DNN will auto-run this code before anything else
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class StartupDnn : IServiceRouteMapper
    {
        /// <summary>
        /// This will be called by DNN when loading the assemblies.
        /// We just want to trigger the DI-Configure
        /// </summary>
        /// <param name="mapRouteManager"></param>
        public void RegisterRoutes(IMapRoute mapRouteManager) => Configure();


        private static bool _alreadyConfigured;

        /// <summary>
        /// Configure IoC for 2sxc. If it's already configured, do nothing.
        /// </summary>
        public void Configure()
        {
            if (_alreadyConfigured)
                return;

            // this service configuration for DNN7 and on DNN9 it is already happned on special startup
            Di.Register();

            // Configure Newtonsoft Time zone handling
            // Moved here in v12.05 - previously it was in the Pre-Serialization converter
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;


            // now we should be able to instantiate registration of DB
            Eav.Factory.StaticBuild<IDbConfiguration>().ConnectionString = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString;
            var globalConfig = Eav.Factory.StaticBuild<IGlobalConfiguration>();

            globalConfig.GlobalFolder = HostingEnvironment.MapPath(DnnConstants.SysFolderRootVirtual);
            globalConfig.GlobalSiteFolder = "~/Portals/_default/";

            // Load features from configuration
            var sysLoader = Eav.Factory.StaticBuild<SystemLoader>();
            sysLoader.StartUp();

            // 2021-11-16 2dm - experimental, working on moving global/preset data into a normal AppState #PresetInAppState
            sysLoader.Log.Add("Try to load global app-state");
            var globalStateLoader = Eav.Factory.StaticBuild<FileAppStateLoaderWIP>();
            var appState = globalStateLoader.AppState(Eav.Constants.PresetAppId);
            var appsMemCache = Eav.Factory.StaticBuild<IAppsCache>();
            appsMemCache.Add(appState);
            // End experimental #PresetInAppState

            // also register this because of a long DNN issue which was fixed, but we don't know if we're running in another version
            SharpZipLibRedirect.RegisterSharpZipLibRedirect();

            // Help RazorBlade to have a proper best-practices ToJson
            // New v12.05
            Razor.Internals.StartUp.RegisterToJson(JsonConvert.SerializeObject);

            _alreadyConfigured = true;
        }
    }
}