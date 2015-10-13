using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;
using ToSic.Eav.Implementations.ValueConverter;
using ToSic.SexyContent.EAV.Implementation.ValueConverter;

namespace ToSic.SexyContent
{
    public class UnityConfig
    {
        private static bool _alreadyConfigured = false;

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