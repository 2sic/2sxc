using ToSic.Eav.Code.Help;
using ToSic.Sxc.Code.Internal.CodeErrorHelp;

namespace ToSic.Sxc.Code.Internal.SourceCode;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CodeFileInfo
{
    private CodeFileInfo(string inherits, CodeFileTypes type, List<CodeHelp> help, bool thisApp = false, string sourceCode = default, string relativePath = default, string fullPath = default)
    {
        Inherits = inherits;
        Type = type;
        Help = help ?? [];
        ThisApp = thisApp;
        SourceCode = sourceCode;
        RelativePath = relativePath;
        FullPath = fullPath;
    }

    internal CodeFileInfo(CodeFileInfo original, string sourceCode, string relativePath = default, string fullPath = default)
    {
        Inherits = original.Inherits;
        Type = original.Type;
        Help = original.Help;
        ThisApp = original.ThisApp;
        SourceCode = sourceCode;
        RelativePath = relativePath ?? original.RelativePath;
        FullPath = fullPath ?? original.FullPath;
    }

    public string Inherits { get; }
    public string SourceCode { get; }
    public string RelativePath { get; }
    public string FullPath { get; }
    public CodeFileTypes Type { get; }
    public List<CodeHelp> Help { get; }
    public bool ThisApp { get; }

    // without base class
    public static CodeFileInfo CodeFileUnknown(string sourceCode, string relativePath = default, string fullPath = default) 
        => new("unknown", CodeFileTypes.Unknown, HelpForRazorCompileErrors.CompileUnknown, sourceCode: sourceCode, relativePath: relativePath, fullPath: fullPath);
    public static CodeFileInfo CodeFileUnknownWithThisApp(string sourceCode, string relativePath = default, string fullPath = default) 
        => new ("unknown", CodeFileTypes.Unknown, HelpForRazorCompileErrors.CompileUnknown, true, sourceCode: sourceCode, relativePath: relativePath, fullPath: fullPath);

    // with some other base class
    public static CodeFileInfo CodeFileOther(string sourceCode, string relativePath = default, string fullPath = default) 
        => new ("other", CodeFileTypes.Other, HelpForRazorCompileErrors.CompileUnknown, sourceCode: sourceCode);
    public static CodeFileInfo CodeFileOtherWithThisApp(string sourceCode, string relativePath = default, string fullPath = default) 
        => new ("other", CodeFileTypes.Other, HelpForRazorCompileErrors.CompileUnknown, true, sourceCode: sourceCode, relativePath: relativePath, fullPath: fullPath);

    public static CodeFileInfo CodeFileNotFound = new("", CodeFileTypes.FileNotFound, []);

    /// <summary>
    /// Template CodeFile objects for different types of files.
    /// They don't contain the source code, which would be added later if needed.
    /// </summary>
    internal static List<CodeFileInfo> CodeFileInfoTemplates =
    [
        CodeFileUnknown(null),
        CodeFileUnknownWithThisApp(null),
        CodeFileOther(null),
        CodeFileOtherWithThisApp(null),
        // cshtml
        new("Custom.Hybrid.Razor12", CodeFileTypes.V12, HelpForRazor12.Compile12, sourceCode: null),
        new("Custom.Hybrid.Razor14", CodeFileTypes.V14, HelpForRazor14.Compile14, sourceCode: null),
        new("Custom.Hybrid.RazorTyped", CodeFileTypes.V16, HelpForRazorTyped.Compile16, sourceCode: null),
        new("Custom.Hybrid.Razor12", CodeFileTypes.V12, HelpForRazor12.Compile12, true, sourceCode: null),
        new("Custom.Hybrid.Razor14", CodeFileTypes.V14, HelpForRazor14.Compile14, true, sourceCode: null),
        new("Custom.Hybrid.RazorTyped", CodeFileTypes.V16, HelpForRazorTyped.Compile16, true, sourceCode: null),
        // c#
        new("Custom.Hybrid.Code12", CodeFileTypes.V12, HelpForRazor12.Compile12, sourceCode: null),
        new("Custom.Hybrid.Code14", CodeFileTypes.V14, HelpForRazor14.Compile14, sourceCode: null),
        new("Custom.Hybrid.CodeTyped", CodeFileTypes.V16, HelpForRazorTyped.Compile16, sourceCode: null),
        new("Custom.Hybrid.Code12", CodeFileTypes.V12, HelpForRazor12.Compile12, true, sourceCode: null),
        new("Custom.Hybrid.Code14", CodeFileTypes.V14, HelpForRazor14.Compile14, true, sourceCode: null),
        new("Custom.Hybrid.CodeTyped", CodeFileTypes.V16, HelpForRazorTyped.Compile16, true, sourceCode: null)
    ];

    /// <summary>
    /// Override ToString for better debugging
    /// </summary>
    public override string ToString() => _toString ??= $"{nameof(CodeFileInfo)} - {nameof(RelativePath)}: '{RelativePath}'; {nameof(FullPath)}: '{FullPath}'; {nameof(SourceCode)}: {SourceCode?.Length}; {nameof(Inherits)}: '{Inherits}'; {nameof(Type)}: '{Type}'; {nameof(ThisApp)}: '{ThisApp}'";
    private string _toString;

    /// <summary>
    /// Create a dictionary of the specs for logging
    /// </summary>
    public IDictionary<string, string> ToDictionary() => new Dictionary<string, string>
    {
        { "RelativePath", RelativePath },
        { "FullPath", FullPath },
        { "SourceCode", SourceCode?.Length.ToString() },
        { "Inherits", Inherits },
        { "Type", Type.ToString() },
        { "ThisApp", ThisApp.ToString() },
    };

}