using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code
{
    public interface IDynamicCodeRoot<out TModel, out TKit>: IDynamicCodeRoot, IDynamicCode<TModel, TKit>
        where TModel : class
        where TKit : ServiceKit
    {
    }
}
