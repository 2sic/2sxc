using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal;

namespace ToSic.Sxc.Services.Internal;

/// <summary>
/// Internal special base class for services which link to the dynamic code root
/// </summary>
[PrivateApi]
// #NoEditorBrowsableBecauseOfInheritance
//[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
[method: PrivateApi]
public abstract class ServiceForDynamicCode(string logName)
    : ServiceBase(logName), INeedsDynamicCodeRoot, IHasDynamicCodeRoot, ICanDebug
{
    /// <summary>
    /// Connect to CodeRoot and it's log
    /// </summary>
    /// <param name="codeRoot"></param>
    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public virtual void ConnectToRoot(IDynamicCodeRoot codeRoot) => ConnectToRoot(codeRoot, null);

    /// <summary>
    /// Connect to CodeRoot and a custom log
    /// </summary>
    /// <param name="codeRoot"></param>
    /// <param name="parentLog"></param>
    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void ConnectToRoot(IDynamicCodeRoot codeRoot, ILog parentLog)
    {
        // Avoid unnecessary reconnects
        if (_alreadyConnected) return;
        _alreadyConnected = true;

        // Remember the parent
        _DynCodeRoot = codeRoot;
        // Link the logs
        this.LinkLog(parentLog ?? codeRoot?.Log);
        // report connection in log
        Log.Fn(message: "Linked to Root").Done();
    }
    private bool _alreadyConnected;

    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public virtual IDynamicCodeRoot _DynCodeRoot { get; private set; }

    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public virtual bool Debug { get; set; }
}