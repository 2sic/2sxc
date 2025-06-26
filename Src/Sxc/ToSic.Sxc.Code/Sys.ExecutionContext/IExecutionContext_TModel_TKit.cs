using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Sys.ExecutionContext;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IExecutionContext<out TModel, out TServiceKit>: IExecutionContext, IHasKit<TServiceKit>
    where TModel : class
    where TServiceKit : ServiceKit;