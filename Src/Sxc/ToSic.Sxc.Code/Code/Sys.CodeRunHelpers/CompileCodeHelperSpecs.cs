using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.Sys.CodeRunHelpers;

/// <summary>
/// Helper information for all code helpers, which is often passed around from one helper to another.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class CompileCodeHelperSpecs(IExecutionContext exCtx, bool isRazor, string codeFileName)
{
    public IExecutionContext ExCtx { get; } = exCtx;
    public bool IsRazor { get; } = isRazor;
    public string CodeFileName { get; } = codeFileName;
}