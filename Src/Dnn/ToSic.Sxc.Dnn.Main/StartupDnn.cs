using System.Configuration;
using System.Web.Hosting;
using DotNetNuke.Web.Api;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav;
using ToSic.Eav.Configuration;
using ToSic.SexyContent.Dnn920;
using ToSic.Sxc;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Polymorphism;
using ToSic.Sxc.WebApi;
using Factory = ToSic.Eav.Factory;

namespace ToSic.SexyContent
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

            var appsCache = GetAppsCacheOverride();
            Factory.ActivateNetCoreDi(services =>
            {
                services
                    .AddDnn(appsCache)
                    .AddAdamWebApi<int, int>()
                    .AddSxcWebApi()
                    .AddSxcCore()
                    .AddEav();
                
                // temp polymorphism - later put into AddPolymorphism
                services.TryAddTransient<Koi>();
                services.TryAddTransient<Permissions>();
                
            });

            // now we should be able to instantiate registration of DB
            Factory.StaticBuild<IDbConfiguration>().ConnectionString = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString;
            var globalConfig = Factory.StaticBuild<IGlobalConfiguration>();

            globalConfig.GlobalFolder = HostingEnvironment.MapPath(DnnConstants.SysFolderRootVirtual);

            // also register this because of a long DNN issue which was fixed, but we don't know if we're running in another version
            SharpZipLibRedirect.RegisterSharpZipLibRedirect();

            _alreadyConfigured = true;
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
    }
}