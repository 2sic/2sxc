using ToSic.Eav.Internal.Features;
using ToSic.Eav.StartUp;
using ToSic.Lib.Services;
using ToSic.Sxc.Dnn.Features;

namespace ToSic.Sxc.Dnn.StartUp;

internal class DnnStartUpRegistrations(FeaturesCatalog featuresCatalog)
    : ServiceBase($"{DnnConstants.LogName}.SUpReg", connect: [featuresCatalog]), IStartUpRegistrations
{
    public string NameId => Log.NameId;

    /// <summary>
    /// Register Dnn features before loading
    /// </summary>
    public void Register() => DnnBuiltInFeatures.Register(featuresCatalog);

}