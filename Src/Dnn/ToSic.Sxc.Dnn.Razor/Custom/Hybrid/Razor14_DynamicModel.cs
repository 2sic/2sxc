using ToSic.Sxc.Engines;
using ToSic.Sxc.Engines.Sys;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

abstract partial class Razor14: ISetDynamicModel
{
    /// <inheritdoc cref="IRazor14{TModel,TServiceKit}.DynamicModel"/>
    public dynamic DynamicModel => RzrHlp.DynamicModel;

    [PrivateApi]
    void ISetDynamicModel.SetDynamicModel(ViewDataWithModel viewData) => RzrHlp.SetDynamicModel(viewData);
}