namespace ToSic.Sxc.Code.Internal.CodeRunHelpers;

/// <summary>
/// Helper information for all code helpers, which is often passed around from one helper to another.
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CodeHelperSpecs(ICodeApiService codeApiSvc, bool isRazor, string codeFileName)
{
    public ICodeApiService CodeApiSvc { get; } = codeApiSvc;
    public bool IsRazor { get; } = isRazor;
    public string CodeFileName { get; } = codeFileName;
}