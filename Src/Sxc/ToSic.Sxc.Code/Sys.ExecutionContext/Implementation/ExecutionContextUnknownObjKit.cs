using ToSic.Sxc.Services.Sys;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Sys.ExecutionContext;

internal class ExecutionContextUnknownObjKit(ExecutionContext.Dependencies services, WarnUseOfUnknown<ExecutionContextUnknownObjKit> _)
    : ExecutionContext<object, ServiceKit>(services, LogScopes.Base);