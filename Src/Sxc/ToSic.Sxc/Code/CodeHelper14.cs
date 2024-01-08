using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Code;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CodeHelper14: CodeHelperXxBase
{
    public CodeHelper14(IDynamicCodeRoot codeRoot, bool isRazor, string codeFileName) : base(codeRoot, isRazor, codeFileName,
        $"{SxcLogging.SxcLogName}.C14Hlp")
    {
    }
}