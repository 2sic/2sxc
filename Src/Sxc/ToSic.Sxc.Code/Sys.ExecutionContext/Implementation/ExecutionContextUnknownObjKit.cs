using ToSic.Sxc.Services.Sys;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Sys.ExecutionContext;

// Must stay generic, since it's the open-generic fallback for ExecutionContext<,>
internal class ExecutionContextUnknownObjKit<TModel, TServiceKit>(ExecutionContext.Dependencies services, WarnUseOfUnknown<ExecutionContextUnknown> _)
    : ExecutionContext<TModel, TServiceKit>(services, LogScopes.Base)
    where TModel : class
    where TServiceKit : ServiceKit;