using ToSic.Sxc.Dnn.Features;
using ToSic.Sys.Boot;
using ToSic.Sys.Capabilities.Features;

namespace ToSic.Sxc.Dnn.StartUp;

internal class DnnBootFeaturesRegistration(FeaturesCatalog featuresCatalog)
    : BootProcessBase("DnnFts", bootPhase: BootPhase.Registrations, connect: [featuresCatalog]), IBootProcess
{
    /// <summary>
    /// Register Dnn features before loading
    /// </summary>
    public override void Run() => DnnBuiltInFeatures.Register(featuresCatalog);

}