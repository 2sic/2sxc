using ToSic.Sxc.Engines.Sys;
using ToSic.Sxc.Render.Sys.Specs;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid;

// ReSharper disable once UnusedMember.Global
abstract partial class Razor14: ISetDynamicModel
{
    /// <inheritdoc cref="Custom.Razor.Sys.IRazor14{TModel,TServiceKit}.DynamicModel"/>
    [PublicApi]
    public dynamic DynamicModel => RzrHlp.DynamicModel;

    [PrivateApi]
    void ISetDynamicModel.SetDynamicModel(RenderSpecs viewData) => RzrHlp.SetDynamicModel(viewData);
}