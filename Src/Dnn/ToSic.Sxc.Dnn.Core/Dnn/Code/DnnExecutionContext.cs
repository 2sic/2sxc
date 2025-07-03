using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Sys;
using ExecutionContext = ToSic.Sxc.Sys.ExecutionContext.ExecutionContext;

namespace ToSic.Sxc.Dnn.Code;

/// <summary>
/// The basic DnnDynamicCode without explicitly typed model / kit
/// </summary>
[PrivateApi]
internal class DnnExecutionContext(ExecutionContext.Dependencies services)
    : DnnExecutionContext<object, ServiceKit>(services);