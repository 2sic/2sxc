using DotNetNuke.Web.Api;
using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http.Formatting;
using System.Web.Hosting;
using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Lib.DI;
using ToSic.Eav.Run;
using ToSic.Eav.Serialization;
using ToSic.Eav.WebApi.Serialization;
using ToSic.Sxc.Images.ImageflowRewrite;
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
            // In some cases this may be called 2x - so we must avoid doing it again
            if (_alreadyConfigured) return;

            // Configure Newtonsoft Time zone handling
            // Moved here in v12.05 - previously it was in the Pre-Serialization converter
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

            // System.Text.Json supports ISO 8601-1:2019, including the RFC 3339 profile
            GlobalConfiguration.Configuration.Formatters.Add(JsonFormatters.SystemTextJsonMediaTypeFormatter);
            // Getting the service provider in Configure is tricky business, because
            // of .net core 2.1 bugs
            // ATM it appears that the service provider will get destroyed after startup, so we MUST get an additional one to use here
            var transientSp = DnnStaticDi.GetGlobalServiceProvider();

            // Configure Eav to Json converters for api v15
            JsonFormatters.SystemTextJsonMediaTypeFormatter.JsonSerializerOptions.Converters.Add(transientSp.Build<EavJsonConverter>());

            // now we should be able to instantiate registration of DB
            transientSp.Build<IDbConfiguration>().ConnectionString = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString;
            var globalConfig = transientSp.Build<IGlobalConfiguration>();

            globalConfig.GlobalFolder = HostingEnvironment.MapPath(DnnConstants.SysFolderRootVirtual);
            globalConfig.AssetsVirtualUrl = DnnConstants.SysFolderRootVirtual + "assets/";
            globalConfig.SharedAppsFolder = "~/Portals/_default/" + AppConstants.AppsRootFolder + "/";

            var sxcSysLoader = transientSp.Build<SystemLoader>();
            sxcSysLoader.StartUp();

            // After the SysLoader got the features, we must attach it to an old API which had was public
            // This was used in Mobius etc. to see if features are activated
#pragma warning disable CS0618
            Features.FeaturesFromDi = sxcSysLoader.EavSystemLoader.Features;
#pragma warning restore CS0618

            // Optional registration of query string rewrite functionality implementation for dnn imageflow module
            Imageflow.Dnn.StartUp.RegisterQueryStringRewrite(ImageflowRewrite.QueryStringRewrite);

            _alreadyConfigured = true;
        }
    }
}