using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Code.Internal.CodeRunHelpers;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CodeHelperXxBase: ServiceBase
{
    protected CodeHelperXxBase(ICodeApiService codeRoot, bool isRazor, string codeFileName, string logName) : base(logName)
    {
        CodeRoot = codeRoot;
        this.LinkLog(codeRoot.Log);
        IsRazor = isRazor;
        CodeFileName = codeFileName;
    }
    protected readonly ICodeApiService CodeRoot;
    protected readonly bool IsRazor;
    protected readonly string CodeFileName;


    public IDevTools DevTools => _devTools.Get(() => new DevTools(IsRazor, CodeFileName, Log));
    private readonly GetOnce<IDevTools> _devTools = new();

}