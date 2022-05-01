using DotNetNuke.Web.Api;
using Newtonsoft.Json;
using System.Configuration;
using System.Web.Hosting;
using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Eav.Plumbing;
using ToSic.SexyContent.Dnn920;
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

            // Register Services if this has not happened yet
            // In Dnn9.4+ this was already done before
            // In older Dnn this didn't happen yet, so this is the latest it can happen
            
            // TODO: STV
            // var newServiceProvider = DnnDi.GetPreparedServiceProvider?.Invoke();


            DnnDi.RegisterServices(null);

            // Now activate the Service Provider, because some Dnn code still needs the static implementation
            DnnStaticDi.StaticDiReady();



            // Configure Newtonsoft Time zone handling
            // Moved here in v12.05 - previously it was in the Pre-Serialization converter
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

            // Getting the service provider in Configure is tricky business, because
            // of .net core 2.1 bugs
            // ATM it appears that the service provider will get destroyed after startup, so we MUST get an additional one to use here
            var transientSp = DnnStaticDi.GetGlobalServiceProvider();// .GetPageScopedServiceProvider();

            // now we should be able to instantiate registration of DB
            transientSp.Build<IDbConfiguration>().ConnectionString = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString;
            var globalConfig = transientSp.Build<IGlobalConfiguration>();

            globalConfig.GlobalFolder = HostingEnvironment.MapPath(DnnConstants.SysFolderRootVirtual);
            globalConfig.AssetsVirtualUrl = DnnConstants.SysFolderRootVirtual + "assets/";
            globalConfig.SharedAppsFolder = "~/Portals/_default/" + AppConstants.AppsRootFolder + "/";

            // Register Sxc features before loading
            Sxc.Configuration.Features.BuiltInFeatures.Register(transientSp.Build<FeaturesCatalog>());

            // Load features from configuration
            var sysLoader = transientSp.Build<SystemLoader>();
            sysLoader.StartUp();

            // After the SysLoader got the features, we must attach it to an old API which had was public
            // This was used in Mobius etc. to see if features are activated
#pragma warning disable CS0618
            Features.FeaturesFromDi = sysLoader.Features;
#pragma warning restore CS0618

            // also register this because of a long DNN issue which was fixed, but we don't know if we're running in another version
            SharpZipLibRedirect.RegisterSharpZipLibRedirect();

            // Help RazorBlade to have a proper best-practices ToJson
            // New v12.05
            // 2022-02-01 2dm - should not be necessary any more, .net Framework doesn't need this
            // But we'll leave it in, because possibly this function is more reliable than the built in
            Razor.StartUp.StartUp.RegisterToJson(JsonConvert.SerializeObject);

            // Optional registration of query string rewrite functionality implementation for dnn imageflow module
            Imageflow.Dnn.StartUp.RegisterQueryStringRewrite(ImageflowRewrite.QueryStringRewrite);

            _alreadyConfigured = true;
        }
    }
}