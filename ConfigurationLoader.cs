using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Implementations.ValueConverter;
using ToSic.Eav.ImportExport.Persistence.File;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.Eav.Plumbing.Booting;
using ToSic.SexyContent.Dnn920;
using ToSic.Sxc.Adam;
using ToSic.SexyContent.Environment;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Environment.Dnn7.EavImplementation;
using ToSic.SexyContent.Environment.Dnn7.Installation;
using ToSic.SexyContent.Environment.Dnn7.ValueProviders;
using ToSic.SexyContent.ImportExport;
using ToSic.SexyContent.Interfaces;
using ToSic.Sxc.Interfaces;

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
        }


        private static void ConfigureIoC()
        {
            Eav.Factory.ActivateNetCoreDi(sc =>
            {
                sc.AddTransient<Eav.Serializers.Serializer, Serializers.Serializer>();
                sc.AddTransient<IEavValueConverter, DnnValueConverter>();
                sc.AddTransient<IUser, DnnUser>();

                sc.AddTransient<XmlExporter, DnnXmlExporter>();
                sc.AddTransient<IImportExportEnvironment, ImportExportEnvironment>();

                sc.AddTransient<IRuntime, Runtime>();
                sc.AddTransient<IEnvironment, DnnEnvironment>();

                sc.AddTransient<IClientDependencyManager, ClientDependencyManager>();
                sc.AddTransient<IEnvironmentFactory, DnnEnvironmentFactory>();
                sc.AddTransient<IWebFactoryTemp, DnnEnvironmentFactory>();
                sc.AddTransient<IRenderingHelpers, DnnRenderingHelpers>();
                sc.AddTransient<IMapAppToInstance, DnnMapAppToInstance>();
                sc.AddTransient<IEnvironmentInstaller, InstallationController>();
                sc.AddTransient<IEnvironmentFileSystem, DnnFileSystem>();
                sc.AddTransient<IEnvironmentValueProviders, DnnValueProviders>();

                new Eav.DependencyInjection().ConfigureNetCoreContainer(sc);
            });
        }
    }
}