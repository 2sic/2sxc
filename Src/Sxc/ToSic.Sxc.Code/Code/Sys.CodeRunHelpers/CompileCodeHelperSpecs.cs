using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.Sys.CodeRunHelpers;

/// <summary>
/// Helper information for all code helpers, which is often passed around from one helper to another.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public record CompileCodeHelperSpecs(
    IExecutionContext ExCtx,
    bool IsRazor,
    string CodeFileName
);