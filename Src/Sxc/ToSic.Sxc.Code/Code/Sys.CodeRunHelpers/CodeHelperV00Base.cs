using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.Sys.CodeRunHelpers;

/// <summary>
/// Base class for version-specific code helpers.
/// </summary>
/// <param name="helperSpecs"></param>
/// <param name="logName"></param>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class CodeHelperV00Base(CompileCodeHelperSpecs helperSpecs, string logName)
    : HelperBase(helperSpecs.ExCtx.Log, logName)
{
    protected IExecutionContext ExCtx => Specs.ExCtx;

    protected CompileCodeHelperSpecs Specs { get; } = helperSpecs;


    public IDevTools DevTools => _devTools.Get(() => new DevTools(Specs, Log))!;
    private readonly GetOnce<IDevTools> _devTools = new();

}