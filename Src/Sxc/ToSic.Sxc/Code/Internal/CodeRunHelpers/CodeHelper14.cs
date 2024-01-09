using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Code.Internal.CodeRunHelpers;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CodeHelper14(IDynamicCodeRoot codeRoot, bool isRazor, string codeFileName)
    : CodeHelperXxBase(codeRoot, isRazor, codeFileName,
        $"{SxcLogging.SxcLogName}.C14Hlp");