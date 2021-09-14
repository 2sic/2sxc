using ToSic.Sxc.Data;

namespace Custom.Hybrid
{
    public partial class Razor12<TModel>
    {
        public dynamic DynamicModel => _dynamicModel ??= new DynamicReadObject(Model, false);
        private dynamic _dynamicModel;
    }
}
