using ToSic.Lib.Helpers;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Code.Internal.CodeRunHelpers;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CodeHelperBase : ServiceForDynamicCode
{
    protected CodeHelperBase(string logName) : base(logName)
    {
    }

    public override void ConnectToRoot(ICodeApiService codeRoot)
    {
        base.ConnectToRoot(codeRoot);
        // Make sure the Code-Log is reset, in case it was used before this call
        _codeLog.Reset();
    }


    #region CodeLog / Html Helper

    public ICodeLog CodeLog => _codeLog.Get(() => new CodeLog(Log));
    private readonly GetOnce<ICodeLog> _codeLog = new();

    #endregion
}