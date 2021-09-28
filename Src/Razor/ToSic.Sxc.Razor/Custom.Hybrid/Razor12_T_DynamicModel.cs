using ToSic.Sxc.Data;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public partial class Razor12<TModel>
    {
        public dynamic DynamicModel => _dynamicModel ??= new DynamicReadObject(Model, true, false);
        private dynamic _dynamicModel;
    }
}
