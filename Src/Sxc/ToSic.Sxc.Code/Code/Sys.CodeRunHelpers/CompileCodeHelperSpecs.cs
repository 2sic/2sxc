using ToSic.Sxc.Code.Context.Sys;


namespace ToSic.Sxc.Code.Sys.CodeRunHelpers;

/// <summary>
/// Helper information for all code helpers, which is often passed around from one helper to another.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public record CompileCodeHelperSpecs(
    ICodeContext ExCtx,
    bool IsRazor,
    string CodeFileName
);