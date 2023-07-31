using ToSic.Lib.Documentation;
using ToSic.Sxc.Engines;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract partial class Razor14: ISetDynamicModel
    {
        /// <inheritdoc cref="IRazor14{TModel,TServiceKit}.DynamicModel"/>
        public dynamic DynamicModel => SysHlp.DynamicModel;

        [PrivateApi]
        void ISetDynamicModel.SetDynamicModel(object data) => SysHlp.SetDynamicModel(data);
    }
}
