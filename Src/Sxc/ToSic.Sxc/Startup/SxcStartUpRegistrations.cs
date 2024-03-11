using ToSic.Eav.Internal.Features;
using ToSic.Eav.StartUp;
using ToSic.Lib.Services;
using ToSic.Sxc.Configuration.Internal;

namespace ToSic.Sxc.Startup;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class SxcStartUpRegistrations(FeaturesCatalog featuresCatalog)
    : ServiceBase($"{SxcLogName}.SUpReg"), IStartUpRegistrations
{
    public string NameId => Log.NameId;

    /// <summary>
    /// Register Dnn features before loading
    /// </summary>
    public void Register() => SxcFeatures.Register(featuresCatalog);

}