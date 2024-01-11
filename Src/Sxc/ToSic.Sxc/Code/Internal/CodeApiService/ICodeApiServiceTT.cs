using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface ICodeApiService<out TModel, out TServiceKit>: ICodeApiService, IHasKit<TServiceKit>
    where TModel : class
    where TServiceKit : ServiceKit
{
}