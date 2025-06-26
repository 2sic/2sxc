using ToSic.Sys.Boot;
using ToSic.Sys.Capabilities.Features;

namespace ToSic.Sxc.Sys.Configuration;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class SxcBootFeaturesRegistrations(FeaturesCatalog featuresCatalog)
    : BootProcessBase($"{SxcLogName}.SUpReg", bootPhase: BootPhase.Registrations, connect: [featuresCatalog]), IBootProcess
{
    /// <summary>
    /// Register Dnn features before loading
    /// </summary>
    public override void Run() => SxcFeatures.Register(featuresCatalog);

}