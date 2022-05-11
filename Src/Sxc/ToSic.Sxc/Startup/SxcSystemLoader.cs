using ToSic.Eav.Configuration;

namespace ToSic.Sxc.Startup
{
    public class SxcSystemLoader
    {

        public SxcSystemLoader(FeaturesCatalog featuresCatalog, SystemLoader systemLoader)
        {
            _featuresCatalog = featuresCatalog;
            SystemLoader = systemLoader;
        }
        private readonly FeaturesCatalog _featuresCatalog;
        public readonly SystemLoader SystemLoader;

        public void StartUp()
        {
            PreStartUp();
            SystemLoader.StartUp();
        }

        public void PreStartUp()
        {
            // Register Sxc features before loading
            Configuration.Features.BuiltInFeatures.Register(_featuresCatalog);
        }
    }
}
