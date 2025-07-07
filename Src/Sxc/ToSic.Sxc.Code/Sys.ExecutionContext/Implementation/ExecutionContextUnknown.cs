using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Sys;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Sys.ExecutionContext;

internal class ExecutionContextUnknown(ExecutionContext.Dependencies services, WarnUseOfUnknown<ExecutionContextUnknown> _)
    : ExecutionContext<object, ServiceKit>(services, LogScopes.Base);