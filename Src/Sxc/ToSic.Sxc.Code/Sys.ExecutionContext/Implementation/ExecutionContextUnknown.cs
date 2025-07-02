using ToSic.Sxc.Services;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Sys.ExecutionContext;

internal class ExecutionContextUnknown(ExecutionContext.MyServices services, WarnUseOfUnknown<ExecutionContextUnknown> _)
    : ExecutionContext<object, ServiceKit>(services, LogScopes.Base);