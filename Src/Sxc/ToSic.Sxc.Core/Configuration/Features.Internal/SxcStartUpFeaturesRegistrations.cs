using ToSic.Eav.Internal.Features;
using ToSic.Eav.StartUp;
using ToSic.Lib.Services;
using ToSic.Sxc.Configuration.Internal;

namespace ToSic.Sxc.Startup;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class SxcStartUpFeaturesRegistrations(FeaturesCatalog featuresCatalog)
    : ServiceBase($"{SxcLogName}.SUpReg"), IStartUpRegistrations
{
    public string NameId => Log.NameId;

    /// <summary>
    /// Register Dnn features before loading
    /// </summary>
    public void Register() => SxcFeatures.Register(featuresCatalog);

}