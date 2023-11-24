using ToSic.Eav.Internal.Features;
using ToSic.Eav.Run;
using ToSic.Eav.StartUp;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Startup;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class SxcStartUpRegistrations: ServiceBase, IStartUpRegistrations
{
    public string NameId => Log.NameId;

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