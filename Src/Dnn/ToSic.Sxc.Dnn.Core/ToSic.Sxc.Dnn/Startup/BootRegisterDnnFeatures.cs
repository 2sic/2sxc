using ToSic.Sxc.Dnn.Features;
using ToSic.Sys.Boot;
using ToSic.Sys.Capabilities.Features;

namespace ToSic.Sxc.Dnn.StartUp;

internal class BootRegisterDnnFeatures(FeaturesCatalog featuresCatalog)
    : ServiceBase("DnnFts", connect: [featuresCatalog]), IBootProcess
{

    public string NameId => Log.NameId;

    public BootPhase Phase => BootPhase.Registrations;

    public int Priority => 999;

    /// <summary>
    /// Register Dnn features before loading
    /// </summary>
    public void Run() => featuresCatalog.Register(DnnFeatures);

    public static readonly Feature[] DnnFeatures =
    [
        DnnBuiltInFeatures.DnnPageWorkflow,
    ];
}