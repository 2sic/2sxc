using ToSic.Sxc.Code.Context.Sys;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Services.Sys;

namespace ToSic.Sxc.Dnn.ExecutionContext.Sys;

[PrivateApi]
internal class DnnExecutionContext<TModel, TServiceKit>(Sxc.Code.Context.Sys.ExecutionContext.Dependencies services)
    : ExecutionContext<TModel, TServiceKit>(services, DnnConstants.LogName), IHasDnn
    where TModel : class
    where TServiceKit : ServiceKit
{
    /// <summary>
    /// Dnn context with module, page, portal etc.
    /// </summary>
    public IDnnContext Dnn => field ??= GetService<IDnnContext>(reuse: true);
}