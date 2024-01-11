using ToSic.Eav.Code.Help;
using ToSic.Sxc.Code.Internal.CodeErrorHelp;

namespace ToSic.Sxc.Code.Internal.SourceCode;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CodeFileInfo
{
    private CodeFileInfo(string inherits, CodeFileTypes type, List<CodeHelp> help, bool thisApp = false)
    {
        Inherits = inherits;
        Type = type;
        Help = help ?? [];
        ThisApp = thisApp;
    }

    public string Inherits { get; }

    public CodeFileTypes Type { get; }
    public List<CodeHelp> Help { get; }
    public bool ThisApp { get; }

    // without base class
    public static CodeFileInfo CodeFileUnknown = new("unknown", CodeFileTypes.Unknown, HelpForRazorCompileErrors.CompileUnknown);
    public static CodeFileInfo CodeFileUnknownWithThisAppCode = new("unknown", CodeFileTypes.Unknown, HelpForRazorCompileErrors.CompileUnknown, true);

    // with some other base class
    public static CodeFileInfo CodeFileOther = new("other", CodeFileTypes.Other, HelpForRazorCompileErrors.CompileUnknown);
    public static CodeFileInfo CodeFileOtherWithThisAppCode = new("other", CodeFileTypes.Other, HelpForRazorCompileErrors.CompileUnknown, true);

    public static CodeFileInfo CodeFileNotFound = new("", CodeFileTypes.FileNotFound, []);

    public static List<CodeFileInfo> CodeFileList =
    [
        CodeFileUnknown,
        CodeFileUnknownWithThisAppCode,
        CodeFileOther,
        CodeFileOtherWithThisAppCode,
        // cshtml
        new("Custom.Hybrid.Razor12", CodeFileTypes.V12, HelpForRazor12.Compile12),
        new("Custom.Hybrid.Razor14", CodeFileTypes.V14, HelpForRazor14.Compile14),
        new("Custom.Hybrid.RazorTyped", CodeFileTypes.V16, HelpForRazorTyped.Compile16),
        new("Custom.Hybrid.Razor12", CodeFileTypes.V12, HelpForRazor12.Compile12, true),
        new("Custom.Hybrid.Razor14", CodeFileTypes.V14, HelpForRazor14.Compile14, true),
        new("Custom.Hybrid.RazorTyped", CodeFileTypes.V16, HelpForRazorTyped.Compile16, true),
        // c#
        new("Custom.Hybrid.Code12", CodeFileTypes.V12, HelpForRazor12.Compile12),
        new("Custom.Hybrid.Code14", CodeFileTypes.V14, HelpForRazor14.Compile14),
        new("Custom.Hybrid.CodeTyped", CodeFileTypes.V16, HelpForRazorTyped.Compile16),
        new("Custom.Hybrid.Code12", CodeFileTypes.V12, HelpForRazor12.Compile12, true),
        new("Custom.Hybrid.Code14", CodeFileTypes.V14, HelpForRazor14.Compile14, true),
        new("Custom.Hybrid.CodeTyped", CodeFileTypes.V16, HelpForRazorTyped.Compile16, true)
    ];
}