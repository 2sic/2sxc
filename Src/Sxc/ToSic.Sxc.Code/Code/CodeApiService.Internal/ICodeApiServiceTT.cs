using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ICodeApiService<out TModel, out TServiceKit>: ICodeApiService, IHasKit<TServiceKit>
    where TModel : class
    where TServiceKit : ServiceKit
{
}