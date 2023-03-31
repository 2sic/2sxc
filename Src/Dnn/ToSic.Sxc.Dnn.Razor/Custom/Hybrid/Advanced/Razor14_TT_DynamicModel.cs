using ToSic.Sxc.Data;
using ToSic.Sxc.Engines;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid.Advanced
{
    public abstract partial class Razor14<TModel, TServiceKit>: ISetDynamicModel
    {
        /// <summary>
        /// Dynamic object containing parameters. So in Dnn it contains the PageData, in Oqtane it contains the Model
        /// </summary>
        /// <remarks>
        /// New in v12
        /// </remarks>
        public dynamic DynamicModel => _dynamicModel ?? (_dynamicModel = new DynamicReadDictionary<object, dynamic>(PageData));
        private dynamic _dynamicModel;

        void ISetDynamicModel.SetDynamicModel(object data) => _dynamicModel = new DynamicReadObject(data, false, false);

    }
}
