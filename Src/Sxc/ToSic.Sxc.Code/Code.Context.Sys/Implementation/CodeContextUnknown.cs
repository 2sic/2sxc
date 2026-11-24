using ToSic.Sxc.Services.Sys;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Code.Context.Sys;

internal class CodeContextUnknown(CodeContext.Dependencies services, WarnUseOfUnknown<CodeContextUnknown> _)
    : CodeContext<object, ServiceKit>(services, LogScopes.Base);