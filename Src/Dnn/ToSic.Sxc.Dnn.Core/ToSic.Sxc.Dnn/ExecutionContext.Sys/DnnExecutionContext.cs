using ToSic.Sxc.Services.Sys;

namespace ToSic.Sxc.Dnn.ExecutionContext.Sys;

/// <summary>
/// The basic DnnDynamicCode without explicitly typed model / kit
/// </summary>
[PrivateApi]
internal class DnnExecutionContext(Sxc.Sys.ExecutionContext.ExecutionContext.Dependencies services)
    : DnnExecutionContext<object, ServiceKit>(services);