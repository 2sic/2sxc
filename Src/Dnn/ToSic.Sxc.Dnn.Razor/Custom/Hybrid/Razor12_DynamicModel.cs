using ToSic.Sxc.Engines;
using ToSic.Sxc.Engines.Sys;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

partial class Razor12: ISetDynamicModel
{
    [PublicApi]
    /// <inheritdoc cref="IRazor14{TModel,TServiceKit}.DynamicModel"/>
    public dynamic DynamicModel => RzrHlp.DynamicModel;

    [PrivateApi]
    void ISetDynamicModel.SetDynamicModel(ViewDataWithModel viewData) => RzrHlp.SetDynamicModel(viewData);
}