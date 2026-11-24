using ToSic.Sxc.Services.Sys;

namespace ToSic.Sxc.Dnn.ExecutionContext.Sys;

/// <summary>
/// The basic DnnDynamicCode without explicitly typed model / kit
/// </summary>
[PrivateApi]
internal class DnnCodeContext(Sxc.Code.Context.Sys.CodeContext.Dependencies services)
    : DnnCodeContext<object, ServiceKit>(services);