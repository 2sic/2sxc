using System.Threading;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.LookUp;
using ToSic.Eav.ValueProviders;
using ToSic.SexyContent.Interfaces;
using ToSic.Sxc.Interfaces;

namespace ToSic.SexyContent.Environment.Dnn7.ValueProviders
{
    public class DnnValueProviders : IEnvironmentValueProviders
    {

        public TokenListFiller GetProviders(int instanceId)
        {
            var providers = new TokenListFiller();
            var portalSettings = PortalSettings.Current;

            if (portalSettings == null) return providers;

            var dnnUsr = portalSettings.UserInfo;
            var dnnCult = Thread.CurrentThread.CurrentCulture;
            var dnn = new TokenReplaceDnn(instanceId, portalSettings, dnnUsr);
            var stdSources = dnn.PropertySources;
            foreach (var propertyAccess in stdSources)
                providers.Sources.Add(propertyAccess.Key,
                    new LookUpInDnnPropertyAccess(propertyAccess.Key, propertyAccess.Value, dnnUsr, dnnCult));
            return providers;
        }
    }
}
