using Microsoft.Practices.Unity;
using ToSic.Eav.Implementations.UserInformation;
using ToSic.Eav.Implementations.ValueConverter;
using ToSic.Eav.ImportExport;
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

            var cont = Eav.Factory.Container;
            new Eav.Configuration().ConfigureDefaultMappings(cont);
            cont.RegisterType(typeof(Eav.Serializers.Serializer), typeof(Serializers.Serializer), new InjectionConstructor());//, null, null, null);
            cont.RegisterType(typeof(IEavValueConverter), typeof(DnnValueConverter), new InjectionConstructor());
            cont.RegisterType(typeof(IEavUserInformation), typeof(DnnUserInformation), new InjectionConstructor());

            cont.RegisterType(typeof(XmlExporter), typeof(ToSxcXmlExporter), new InjectionConstructor());
            cont.RegisterType(typeof(IImportExportEnvironment), typeof(ImportExportEnvironment), new InjectionConstructor());

            _alreadyConfigured = true;
        }
    }
}