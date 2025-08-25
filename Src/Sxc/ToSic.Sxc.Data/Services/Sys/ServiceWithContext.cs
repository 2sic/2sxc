using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Services.Sys;

/// <summary>
/// Internal special base class for services which link to the dynamic code root
/// </summary>
[PrivateApi]
// #NoEditorBrowsableBecauseOfInheritance
//[ShowApiWhenReleased(ShowApiMode.Never)]
[method: PrivateApi]
public abstract class ServiceWithContext(string logName, NoParamOrder protect = default, object[]? connect = default)
    : ServiceBase(logName, protect: protect, connect: connect), INeedsExecutionContext, ICanDebug
{
    /// <summary>
    /// Connect to CodeRoot and it's log
    /// </summary>
    /// <param name="exCtx"></param>
    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public virtual void ConnectToRoot(IExecutionContext exCtx)
        => ConnectToRoot(exCtx, null);

    /// <summary>
    /// Connect to CodeRoot and a custom log
    /// </summary>
    /// <param name="exCtx"></param>
    /// <param name="parentLog"></param>
    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public void ConnectToRoot(IExecutionContext? exCtx, ILog? parentLog)
    {
        // Avoid unnecessary reconnects
        if (_alreadyConnected)
            return;
        _alreadyConnected = true;

        // Remember the parent
        ExCtxOrNull = exCtx;
        // Link the logs
        this.LinkLog(parentLog ?? exCtx?.Log);
        // report connection in log
        Log.Fn(message: "Linked to Root").Done();
    }
    private bool _alreadyConnected;

    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]

    protected IExecutionContext ExCtx => ExCtxOrNull
                                         ?? throw new($"{nameof(ExCtxOrNull)} is null - this is a bug, please report it.");

    protected IExecutionContext? ExCtxOrNull { get; private set; }

    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public virtual bool Debug { get; set; }
}