using ToSic.Sxc.Code.Sys.CodeRunHelpers;

namespace ToSic.Sxc.Code.Internal.CodeRunHelpers;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class CodeHelperV14(CodeHelperSpecs helperSpecs) : CodeHelperV00Base(helperSpecs, $"{SxcLogName}.C14Hlp");