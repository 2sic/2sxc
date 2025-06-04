using ToSic.Lib.Services;
using ToSic.Sxc.Services;
#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Code.Internal;

internal class ExecutionContextUnknown<TModel, TServiceKit>(ExecutionContext.MyServices services, WarnUseOfUnknown<ExecutionContextUnknown> _)
    : ExecutionContext<object, ServiceKit>(services, LogScopes.Base);