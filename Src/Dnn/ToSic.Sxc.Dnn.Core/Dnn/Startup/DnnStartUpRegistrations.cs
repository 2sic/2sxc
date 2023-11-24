using ToSic.Eav.Internal.Features;
using ToSic.Eav.StartUp;
using ToSic.Lib.Services;
using ToSic.Sxc.Dnn.Features;

namespace ToSic.Sxc.Dnn.StartUp
{
    public class DnnStartUpRegistrations: ServiceBase, IStartUpRegistrations
    {
        public string NameId => Log.NameId;

        public DnnStartUpRegistrations(FeaturesCatalog featuresCatalog): base($"{DnnConstants.LogName}.SUpReg")
        {
            ConnectServices(
                _featuresCatalog = featuresCatalog
            );
        }
        private readonly FeaturesCatalog _featuresCatalog;

        /// <summary>
        /// Register Dnn features before loading
        /// </summary>
        public void Register() => DnnBuiltInFeatures.Register(_featuresCatalog);

    }
}