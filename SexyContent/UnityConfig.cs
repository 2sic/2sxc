using Microsoft.Practices.Unity;
using ToSic.Eav.Implementations.ValueConverter;
using ToSic.SexyContent.EAV.Implementation.ValueConverter;

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
            cont.RegisterType(typeof(IEavValueConverter), typeof(SexyContentValueConverter), new InjectionConstructor());
            _alreadyConfigured = true;
        }
    }
}