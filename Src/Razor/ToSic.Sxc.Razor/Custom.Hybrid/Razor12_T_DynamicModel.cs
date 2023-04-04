using ToSic.Sxc.Data;
using ToSic.Sxc.Engines;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public partial class Razor12<TModel>: ISetDynamicModel
    {
        public dynamic DynamicModel => _dynamicModel ??= new DynamicReadObject(Model, true, false);
        private dynamic _dynamicModel;

        void ISetDynamicModel.SetDynamicModel(object data) => _dynamicModel = new DynamicReadObject(data, false, false);

    }
}
