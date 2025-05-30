using ToSic.Lib.Boot;
using ToSic.Sxc.Configuration.Internal;
using ToSic.Sys.Capabilities.Features;

namespace ToSic.Sxc.Startup;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class SxcBootFeaturesRegistrations(FeaturesCatalog featuresCatalog)
    : BootProcessBase($"{SxcLogName}.SUpReg", bootPhase: BootPhase.Registrations, connect: [featuresCatalog]), IBootProcess
{
    /// <summary>
    /// Register Dnn features before loading
    /// </summary>
    public override void Run() => SxcFeatures.Register(featuresCatalog);

}