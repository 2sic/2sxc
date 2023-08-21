using ToSic.Razor.Internals.Documentation;
using ToSic.Sxc.Engines;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public partial class Razor12: ISetDynamicModel
    {
        /// <inheritdoc cref="IRazor14{TModel,TServiceKit}.DynamicModel"/>
        public dynamic DynamicModel => SysHlp.DynamicModel;

        [PrivateApi]
        void ISetDynamicModel.SetDynamicModel(object data) => SysHlp.SetDynamicModel(data);
    }
}
