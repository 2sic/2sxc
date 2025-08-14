using ToSic.Sxc.Engines.Sys;
using ToSic.Sxc.Render.Sys.Specs;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

partial class Razor12: ISetDynamicModel
{
    /// <inheritdoc cref="Custom.Razor.Sys.IRazor14{TModel,TServiceKit}.DynamicModel"/>
    [PublicApi]
    public dynamic DynamicModel => RzrHlp.DynamicModel;

    [PrivateApi]
    void ISetDynamicModel.SetDynamicModel(RenderSpecs viewData) => RzrHlp.SetDynamicModel(viewData);
}