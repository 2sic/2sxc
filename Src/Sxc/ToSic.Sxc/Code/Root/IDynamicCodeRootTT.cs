using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IDynamicCodeRoot<out TModel, out TServiceKit>: IDynamicCodeRoot, IDynamicCodeKit<TServiceKit>
    where TModel : class
    where TServiceKit : ServiceKit
{
}