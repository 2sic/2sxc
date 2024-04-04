using ToSic.Lib.Helpers;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Code.Internal.CodeRunHelpers;

/// <summary>
/// Base class for version-specific code helpers.
/// </summary>
/// <param name="helperSpecs"></param>
/// <param name="logName"></param>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class CodeHelperV00Base(CodeHelperSpecs helperSpecs, string logName)
    : HelperBase(helperSpecs.CodeApiSvc.Log, logName)
{
    protected ICodeApiService CodeApiSvc => Specs.CodeApiSvc;

    protected CodeHelperSpecs Specs { get; } = helperSpecs;


    public IDevTools DevTools => _devTools.Get(() => new DevTools(Specs, Log));
    private readonly GetOnce<IDevTools> _devTools = new();

}