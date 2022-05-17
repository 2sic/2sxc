using ToSic.Eav.Configuration;
using ToSic.Sxc.Startup;

namespace ToSic.Sxc.Dnn.StartUp
{
    public class DnnSystemLoader
    {

        public DnnSystemLoader(SxcSystemLoader sxcLoader, FeaturesCatalog featuresCatalog)
        {
            _featuresCatalog = featuresCatalog;
            SxcLoader = sxcLoader;
        }
        private readonly FeaturesCatalog _featuresCatalog;
        public readonly SxcSystemLoader SxcLoader;

        public void StartUp()
        {
            PreStartUp();
            SxcLoader.StartUp();
        }

        public void PreStartUp()
        {
            // Register Dnn features before loading
            Configuration.Features.BuiltInFeatures.Register(_featuresCatalog);
        }
    }
}
