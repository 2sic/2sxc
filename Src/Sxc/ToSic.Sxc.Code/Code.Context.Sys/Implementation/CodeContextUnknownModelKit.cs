using ToSic.Sxc.Services.Sys;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Code.Context.Sys;

// Must stay generic, since it's the open-generic fallback for CodeContext<,>
internal class CodeContextUnknownModelKit<TModel, TServiceKit>(CodeContext.Dependencies services, WarnUseOfUnknown<CodeContextUnknown> _)
    : CodeContext<TModel, TServiceKit>(services, LogScopes.Base)
    where TModel : class
    where TServiceKit : ServiceKit;