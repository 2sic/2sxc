using System.Threading;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.ValueProvider;
using ToSic.SexyContent.Interfaces;
using ToSic.Sxc.Interfaces;

namespace ToSic.SexyContent.Environment.Dnn7.ValueProviders
{
    public class DnnValueProviders : IEnvironmentValueProviders
    {

        public ValueCollectionProvider GetProviders(int instanceId)
        {
            var providers = new ValueCollectionProvider();
            var portalSettings = PortalSettings.Current;

            if (portalSettings == null) return providers;

            var dnnUsr = portalSettings.UserInfo;
            var dnnCult = Thread.CurrentThread.CurrentCulture;
            var dnn = new TokenReplaceDnn(instanceId, portalSettings, dnnUsr);
            var stdSources = dnn.PropertySources;
            foreach (var propertyAccess in stdSources)
                providers.Sources.Add(propertyAccess.Key,
                    new ValueProviderWrapperForPropertyAccess(propertyAccess.Key, propertyAccess.Value, dnnUsr, dnnCult));
            return providers;
        }
    }
}
