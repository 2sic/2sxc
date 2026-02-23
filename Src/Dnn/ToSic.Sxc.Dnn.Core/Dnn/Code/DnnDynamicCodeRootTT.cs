using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Sys.ExecutionContext;
using ExecutionContext = ToSic.Sxc.Sys.ExecutionContext.ExecutionContext;

namespace ToSic.Sxc.Dnn.Code;

[PrivateApi]
internal class DnnExecutionContext<TModel, TServiceKit>(ExecutionContext.Dependencies services)
    : ExecutionContext<TModel, TServiceKit>(services, DnnConstants.LogName), IHasDnn
    where TModel : class
    where TServiceKit : ServiceKit
{
    /// <summary>
    /// Dnn context with module, page, portal etc.
    /// </summary>
    public IDnnContext Dnn => field ??= GetService<IDnnContext>(reuse: true);
}