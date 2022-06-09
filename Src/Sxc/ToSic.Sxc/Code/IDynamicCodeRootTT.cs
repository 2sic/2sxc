using ToSic.Sxc.Services.Kits;

namespace ToSic.Sxc.Code
{
    public interface IDynamicCodeRoot<out TModel, out TKit>: IDynamicCodeRoot, IDynamicCode<TModel, TKit>
        where TKit: KitBase
    {
    }
}
