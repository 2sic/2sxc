using ToSic.Lib.Services;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Services.Internal;

/// <summary>
/// Internal special base class for services which link to the dynamic code root
/// </summary>
[PrivateApi]
// #NoEditorBrowsableBecauseOfInheritance
//[ShowApiWhenReleased(ShowApiMode.Never)]
[method: PrivateApi]
public abstract class ServiceForDynamicCode(string logName, NoParamOrder protect = default, bool errorIfNotConnected = false, object[] connect = default)
    : ServiceBase(logName, protect: protect, connect: connect), INeedsCodeApiService, IHasCodeApiService, ICanDebug
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
    public void ConnectToRoot(IExecutionContext exCtx, ILog parentLog)
    {
        // Avoid unnecessary reconnects
        if (_alreadyConnected) return;
        _alreadyConnected = true;

        // Remember the parent
        _CodeApiSvc = exCtx as ICodeApiService;
        // Link the logs
        this.LinkLog(parentLog ?? exCtx?.Log);
        // report connection in log
        Log.Fn(message: "Linked to Root").Done();
    }
    private bool _alreadyConnected;

    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public virtual ICodeApiService _CodeApiSvc { get; private set; }

    protected ICodeApiService CodeApiSvc => _CodeApiSvc
                                            ?? (errorIfNotConnected
                                                ? throw new($"{nameof(CodeApiSvc)} is null")
                                                : null);

    protected IExecutionContext ExCtx => CodeApiSvc;

    protected IExecutionContext ExCtxOrNull => _CodeApiSvc;

    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public virtual bool Debug { get; set; }
}