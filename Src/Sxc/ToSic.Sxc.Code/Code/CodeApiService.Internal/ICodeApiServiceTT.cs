using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IExecutionContext<out TModel, out TServiceKit>: IExecutionContext, IHasKit<TServiceKit>
    where TModel : class
    where TServiceKit : ServiceKit;