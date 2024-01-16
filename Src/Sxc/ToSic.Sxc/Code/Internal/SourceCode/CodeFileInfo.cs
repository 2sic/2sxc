using ToSic.Eav.Code.Help;
using ToSic.Sxc.Code.Internal.CodeErrorHelp;

namespace ToSic.Sxc.Code.Internal.SourceCode;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CodeFileInfo
{
    private CodeFileInfo(string inherits, CodeFileTypes type, List<CodeHelp> help, bool thisApp = false, string sourceCode = default)
    {
        Inherits = inherits;
        Type = type;
        Help = help ?? [];
        ThisApp = thisApp;
        SourceCode = sourceCode;
    }

    public string Inherits { get; }
    public string SourceCode { get; }
    public CodeFileTypes Type { get; }
    public List<CodeHelp> Help { get; }
    public bool ThisApp { get; }

    // without base class
    public static CodeFileInfo CodeFileUnknown(string sourceCode) => new("unknown", CodeFileTypes.Unknown, HelpForRazorCompileErrors.CompileUnknown, sourceCode: sourceCode);
    public static CodeFileInfo CodeFileUnknownWithThisAppCode(string sourceCode) => new ("unknown", CodeFileTypes.Unknown, HelpForRazorCompileErrors.CompileUnknown, true, sourceCode: sourceCode);

    // with some other base class
    public static CodeFileInfo CodeFileOther(string sourceCode) => new ("other", CodeFileTypes.Other, HelpForRazorCompileErrors.CompileUnknown, sourceCode: sourceCode);
    public static CodeFileInfo CodeFileOtherWithThisAppCode(string sourceCode) => new ("other", CodeFileTypes.Other, HelpForRazorCompileErrors.CompileUnknown, true, sourceCode: sourceCode);

    public static CodeFileInfo CodeFileNotFound = new("", CodeFileTypes.FileNotFound, []);

    public static List<CodeFileInfo> CodeFileList(string sourceCode) =>
    [
        CodeFileUnknown(sourceCode),
        CodeFileUnknownWithThisAppCode(sourceCode),
        CodeFileOther(sourceCode),
        CodeFileOtherWithThisAppCode(sourceCode),
        // cshtml
        new("Custom.Hybrid.Razor12", CodeFileTypes.V12, HelpForRazor12.Compile12, sourceCode: sourceCode),
        new("Custom.Hybrid.Razor14", CodeFileTypes.V14, HelpForRazor14.Compile14, sourceCode: sourceCode),
        new("Custom.Hybrid.RazorTyped", CodeFileTypes.V16, HelpForRazorTyped.Compile16, sourceCode: sourceCode),
        new("Custom.Hybrid.Razor12", CodeFileTypes.V12, HelpForRazor12.Compile12, true, sourceCode: sourceCode),
        new("Custom.Hybrid.Razor14", CodeFileTypes.V14, HelpForRazor14.Compile14, true, sourceCode: sourceCode),
        new("Custom.Hybrid.RazorTyped", CodeFileTypes.V16, HelpForRazorTyped.Compile16, true, sourceCode: sourceCode),
        // c#
        new("Custom.Hybrid.Code12", CodeFileTypes.V12, HelpForRazor12.Compile12, sourceCode: sourceCode),
        new("Custom.Hybrid.Code14", CodeFileTypes.V14, HelpForRazor14.Compile14, sourceCode: sourceCode),
        new("Custom.Hybrid.CodeTyped", CodeFileTypes.V16, HelpForRazorTyped.Compile16, sourceCode: sourceCode),
        new("Custom.Hybrid.Code12", CodeFileTypes.V12, HelpForRazor12.Compile12, true, sourceCode: sourceCode),
        new("Custom.Hybrid.Code14", CodeFileTypes.V14, HelpForRazor14.Compile14, true, sourceCode: sourceCode),
        new("Custom.Hybrid.CodeTyped", CodeFileTypes.V16, HelpForRazorTyped.Compile16, true, sourceCode: sourceCode)
    ];
}