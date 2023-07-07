using System.Dynamic;
using ToSic.Sxc.Data;
using ToSic.Sxc.Engines;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract partial class Razor14: ISetDynamicModel
    {
        /// <inheritdoc cref="IRazor14{TModel,TServiceKit}.DynamicModel"/>
        public dynamic DynamicModel => _dynamicModel ?? (_dynamicModel = new DynamicReadDictionary<object, dynamic>(PageData));
        private DynamicObject _dynamicModel;

        void ISetDynamicModel.SetDynamicModel(object data) 
            => _dynamicModel = new DynamicReadObject(data, false, false);
    }


}
