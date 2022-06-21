using ToSic.Sxc.Data;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract partial class Razor14<TModel, TServiceKit>
    {
        /// <summary>
        /// Dynamic object containing parameters. So in Dnn it contains the PageData, in Oqtane it contains the Model
        /// </summary>
        /// <remarks>
        /// New in v12
        /// </remarks>
        public dynamic DynamicModel => _dynamicModel ?? (_dynamicModel = new DynamicReadDictionary<object, dynamic>(PageData));

        private dynamic _dynamicModel;
    }
}
