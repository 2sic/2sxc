using ToSic.Sxc.Dnn.Features;
using ToSic.Sys.Boot;
using ToSic.Sys.Capabilities.Features;

namespace ToSic.Sxc.Dnn.StartUp;

internal class BootRegisterDnnFeatures(FeaturesCatalog featuresCatalog)
    : BootProcessBase("DnnFts", bootPhase: BootPhase.Registrations, connect: [featuresCatalog]), IBootProcess
{
    /// <summary>
    /// Register Dnn features before loading
    /// </summary>
    public override void Run() => featuresCatalog.Register(DnnFeatures);

    public static readonly Feature[] DnnFeatures =
    [
        DnnBuiltInFeatures.DnnPageWorkflow,
    ];
}