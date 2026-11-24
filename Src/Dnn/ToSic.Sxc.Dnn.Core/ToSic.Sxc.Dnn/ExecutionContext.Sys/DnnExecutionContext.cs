using ToSic.Sxc.Services.Sys;

namespace ToSic.Sxc.Dnn.ExecutionContext.Sys;

/// <summary>
/// The basic DnnDynamicCode without explicitly typed model / kit
/// </summary>
[PrivateApi]
internal class DnnExecutionContext(Sxc.Code.Context.Sys.ExecutionContext.Dependencies services)
    : DnnExecutionContext<object, ServiceKit>(services);