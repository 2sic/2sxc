using ToSic.Lib.Documentation;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Razor.Internal;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

abstract partial class Razor14: ISetDynamicModel
{
    /// <inheritdoc cref="IRazor14{TModel,TServiceKit}.DynamicModel"/>
    public dynamic DynamicModel => SysHlp.DynamicModel;

    [PrivateApi]
    void ISetDynamicModel.SetDynamicModel(object data) => SysHlp.SetDynamicModel(data);
}