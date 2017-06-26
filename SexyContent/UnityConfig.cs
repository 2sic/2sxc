using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps.ImportExport;
using ToSic.Eav.Implementations.UserInformation;
using ToSic.Eav.Implementations.ValueConverter;
using ToSic.Eav.Persistence.Interfaces;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Environment.Dnn7.EavImplementation;
using ToSic.SexyContent.ImportExport;

namespace ToSic.SexyContent
{
    /// <summary>
    /// this configures unity (the IoC container)
    /// Never call this directly! always go through Settings.Ensure...
    /// </summary>
    public class UnityConfig
    {
        private static bool _alreadyConfigured;

        /// <summary>
        /// Configure Unity for 2sxc. If it's already configured, do nothing.
        /// </summary>
        public void Configure()
        {
            if (_alreadyConfigured)
                return;

            Eav.Factory.ActivateNetCoreDi(sc =>
            {
                sc.AddTransient<Eav.Serializers.Serializer, Serializers.Serializer>();//new InjectionConstructor());//, null, null, null);
                sc.AddTransient<IEavValueConverter, DnnValueConverter>();//new InjectionConstructor());
                sc.AddTransient<IEavUserInformation, DnnUserInformation>();//new InjectionConstructor());

                sc.AddTransient<XmlExporter, ToSxcXmlExporter>();//(new InjectionConstructor(0, 0, true, new string[0], new string[0]));
                sc.AddTransient<IImportExportEnvironment, ImportExportEnvironment>();//new InjectionConstructor());

                new Eav.DependencyInjection().ConfigureNetCoreContainer(sc);
            });

            _alreadyConfigured = true;
        }
    }
}