using System;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Caching;
using ToSic.Eav.LookUp;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.Eav.Plumbing.Booting;
using ToSic.Eav.Repositories;
using ToSic.Eav.Run;
using ToSic.SexyContent.Dnn920;
using ToSic.Sxc;
using ToSic.Sxc.Adam;
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
using ToSic.Sxc.Web;
using ToSic.Sxc.Polymorphism;
using ToSic.Sxc.Run;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Adam;
using Factory = ToSic.Eav.Factory;

namespace ToSic.SexyContent
{
    /// <inheritdoc />
    /// <summary>
    /// this configures unity (the IoC container)
    /// Never call this directly! always go through Settings.Ensure...
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class ConfigurationLoader: IConfigurationLoader
    {
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