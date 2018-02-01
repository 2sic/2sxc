using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Implementations.UserInformation;
using ToSic.Eav.Implementations.ValueConverter;
using ToSic.Eav.ImportExport.Persistence.File;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.Eav.Plumbing.Booting;
using ToSic.SexyContent.Environment;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Environment.Dnn7.EavImplementation;
using ToSic.SexyContent.Environment.Dnn7.Security;
using ToSic.SexyContent.Environment.Interfaces;
using ToSic.SexyContent.ImportExport;
using ToSic.SexyContent.Interfaces;
using ToSic.SexyContent.Internal;
using ToSic.Sxc.Interfaces;
using Configuration = ToSic.Eav.Configuration;

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
        /// Configure Unity for 2sxc. If it's already configured, do nothing.
        /// </summary>
        public void Configure()
        {
            if (_alreadyConfigured)
                return;

            ConfigureConnectionString();
            ConfigureIoC();

            _alreadyConfigured = true;
        }


        private void ConfigureConnectionString()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString;
            Configuration.SetConnectionString(connectionString);
        }


        private static void ConfigureIoC()
        {
            Eav.Factory.ActivateNetCoreDi(sc =>
            {
                sc.AddTransient<Eav.Serializers.Serializer, SexyContent.Serializers.Serializer>();
                sc.AddTransient<IEavValueConverter, DnnValueConverter>();
                sc.AddTransient<IEavUserInformation, DnnUserInformation>();

                sc.AddTransient<XmlExporter, DnnXmlExporter>();
                sc.AddTransient<IImportExportEnvironment, ImportExportEnvironment>();

                sc.AddTransient<IRuntime, Runtime>();
                sc.AddTransient<IEnvironment, DnnEnvironment>();

                sc.AddTransient<IClientDependencyManager, ClientDependencyManager>();
                sc.AddTransient<IEnvironmentFactory, DnnEnvironmentFactory>();
                sc.AddTransient<IRenderingHelpers, DnnRenderingHelpers>();
                sc.AddTransient<IMapAppToInstance, DnnMapAppToInstance>();
                sc.AddTransient<IPermissions, DnnPermissions>();

                new Eav.DependencyInjection().ConfigureNetCoreContainer(sc);
            });
        }
    }
}