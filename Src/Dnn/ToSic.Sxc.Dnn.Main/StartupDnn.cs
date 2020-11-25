using System.Configuration;
using DotNetNuke.Web.Api;
using ToSic.Eav;
using ToSic.SexyContent.Dnn920;
using ToSic.Sxc;
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
    public class StartUpDnn : IServiceRouteMapper
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

}