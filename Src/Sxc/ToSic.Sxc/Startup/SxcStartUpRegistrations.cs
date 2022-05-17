using ToSic.Eav.Configuration;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Startup
{
    public class SxcStartUpRegistrations: HasLog, IStartUpRegistrations
    {
        public string NameId => Log.Identifier;

        public SxcStartUpRegistrations(FeaturesCatalog featuresCatalog): base($"{Constants.SxcLogName}.SUpReg")
        {
            _featuresCatalog = featuresCatalog;
        }
        private readonly FeaturesCatalog _featuresCatalog;

        /// <summary>
        /// Register Dnn features before loading
        /// </summary>
        public void Register() => Configuration.Features.BuiltInFeatures.Register(_featuresCatalog);

    }
}