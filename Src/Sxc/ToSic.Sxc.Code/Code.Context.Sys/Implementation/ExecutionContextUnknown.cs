using ToSic.Sxc.Services.Sys;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Code.Context.Sys;

internal class ExecutionContextUnknown(ExecutionContext.Dependencies services, WarnUseOfUnknown<ExecutionContextUnknown> _)
    : ExecutionContext<object, ServiceKit>(services, LogScopes.Base);