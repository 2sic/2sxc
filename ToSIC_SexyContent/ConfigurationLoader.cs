using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Configuration;
using ToSic.Eav.Data;
using ToSic.Eav.ImportExport.Persistence.File;
using ToSic.Eav.Interfaces;
using ToSic.Eav.LookUp;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.Eav.Plumbing.Booting;
using ToSic.SexyContent.Dnn920;
using ToSic.Sxc.Adam;
using ToSic.SexyContent.Environment;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Interfaces;
using ToSic.Sxc.Apps.ImportExport;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.ImportExport;
using ToSic.Sxc.Dnn.Install;
using ToSic.Sxc.Dnn.LookUp;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Interfaces;
using ToSic.Sxc.Serializers;

namespace ToSic.SexyContent
{
    /// <inheritdoc />
    /// <summary>
    /// this configures unity (the IoC container)
    /// Never call this directly! always go through Settings.Ensure...
    /// </summary>
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
            ConfigureIoC();
            SharpZipLibRedirect.RegisterSharpZipLibRedirect();

            _alreadyConfigured = true;
        }


        private void ConfigureConnectionString()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString;
            Eav.Repository.Efc.Implementations.Configuration.SetConnectionString(connectionString);
            Eav.Repository.Efc.Implementations.Configuration.SetFeaturesHelpLink("https://2sxc.org/help?tag=features", "https://2sxc.org/r/f/");
        }


        private static void ConfigureIoC()
        {
            Eav.Factory.ActivateNetCoreDi(sc =>
            {
                sc.AddTransient<Eav.Serializers.Serializer, Serializer>();
                sc.AddTransient<IValueConverter, ValueConverter>();
                sc.AddTransient<IUser, DnnUser>();

                sc.AddTransient<XmlExporter, DnnXmlExporter>();
                sc.AddTransient<IImportExportEnvironment, ImportExportEnvironment>();

                sc.AddTransient<IRuntime, Runtime>();
                sc.AddTransient<IAppEnvironment, DnnEnvironment>();
                sc.AddTransient<IEnvironment, DnnEnvironment>();

                // The file-importer - temporarily itself
                sc.AddTransient<XmlImportWithFiles, XmlImportFull>();

                sc.AddTransient<IClientDependencyManager, ClientDependencyManager>();
                sc.AddTransient<IEnvironmentFactory, DnnEnvironmentFactory>();
                sc.AddTransient<IWebFactoryTemp, DnnEnvironmentFactory>();
                sc.AddTransient<IRenderingHelpers, DnnRenderingHelpers>();
                sc.AddTransient<IMapAppToInstance, DnnMapAppToInstance>();
                sc.AddTransient<IEnvironmentInstaller, InstallationController>();
                sc.AddTransient<IEnvironmentFileSystem, DnnFileSystem>();
                sc.AddTransient<IGetEngine, GetDnnEngine>();
                sc.AddTransient<IFingerprintProvider, FingerprintProvider>();

                new Eav.DependencyInjection().ConfigureNetCoreContainer(sc);
            });
        }
    }
}