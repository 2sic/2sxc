using ToSic.Sxc.Code.Context.Sys;

namespace ToSic.Sxc.Services.Sys;

/// <summary>
/// Internal special base class for services which link to the dynamic code root
/// </summary>
[PrivateApi]
// #NoEditorBrowsableBecauseOfInheritance
//[ShowApiWhenReleased(ShowApiMode.Never)]
[method: PrivateApi]
public abstract class ServiceWithContext(string logName, NoParamOrder npo = default, object[]? connect = default)
    : ServiceBase(logName, npo: npo, connect: connect), ICodeContextRequired, ICanDebug
{
    /// <summary>
    /// Connect to CodeRoot and it's log
    /// </summary>
    /// <param name="codeCtx"></param>
    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public virtual void SetupContext(ICodeContext codeCtx)
    {
        // Avoid unnecessary reconnects
        if (_alreadyConnected)
            return;
        _alreadyConnected = true;

        // Remember the parent
        ExCtxOrNull = codeCtx;
        // Link the logs
        this.LinkLog(codeCtx?.Log);
        // report connection in log
        Log.Fn(message: "Linked to Root").Done();
    }
    private bool _alreadyConnected;

    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]

    protected ICodeContext ExCtx => ExCtxOrNull
                                         ?? throw new($"{nameof(ExCtxOrNull)} is null - this is a bug, please report it.");

    protected ICodeContext? ExCtxOrNull { get; private set; }

    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public virtual bool Debug { get; set; }
}