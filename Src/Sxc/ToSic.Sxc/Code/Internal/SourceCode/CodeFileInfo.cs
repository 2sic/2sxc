using ToSic.Eav.Code.Help;
using ToSic.Sxc.Code.Internal.CodeErrorHelp;

namespace ToSic.Sxc.Code.Internal.SourceCode;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CodeFileInfo
{
    private CodeFileInfo(string inherits, CodeFileTypes type, List<CodeHelp> help, bool useAppCode = false, string sourceCode = default, string relativePath = default, string fullPath = default)
    {
        Inherits = inherits;
        Type = type;
        Help = help ?? [];
        AppCode = useAppCode;
        SourceCode = sourceCode;
        RelativePath = relativePath;
        FullPath = fullPath;
    }

    internal CodeFileInfo(CodeFileInfo original, string sourceCode, string relativePath = default, string fullPath = default, bool? useAppCode = default)
    {
        Inherits = original.Inherits;
        Type = original.Type;
        Help = original.Help;
        AppCode = useAppCode ?? original.AppCode;
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
    public bool AppCode { get; }

    // without base class
    public static CodeFileInfo TemplateUnknown = new("unknown", CodeFileTypes.Unknown, HelpForRazorCompileErrors.CompileUnknown);

    // with some other base class
    public static CodeFileInfo TemplateOther = new("other", CodeFileTypes.Other, HelpForRazorCompileErrors.CompileUnknown);

    public static CodeFileInfo CodeFileNotFound = new("", CodeFileTypes.FileNotFound, []);

    public static CodeFileInfo CodeFileInheritsAppCode = new("AppCode.*", CodeFileTypes.V16, HelpForRazorTyped.Compile16, sourceCode: null);
    /// <summary>
    /// Template CodeFile objects for different types of files.
    /// They don't contain the source code, which would be added later if needed.
    /// </summary>
    internal static List<CodeFileInfo> CodeFileInfoTemplates =
    [
        TemplateUnknown,
        TemplateOther,
        // cshtml
        new("Custom.Hybrid.Razor12", CodeFileTypes.V12, HelpForRazor12.Compile12, sourceCode: null),
        new("Custom.Hybrid.Razor14", CodeFileTypes.V14, HelpForRazor14.Compile14, sourceCode: null),
        new("Custom.Hybrid.RazorTyped", CodeFileTypes.V16, HelpForRazorTyped.Compile16, sourceCode: null),

        // c#
        new("Custom.Hybrid.Code12", CodeFileTypes.V12, HelpForRazor12.Compile12, sourceCode: null),
        new("Custom.Hybrid.Code14", CodeFileTypes.V14, HelpForRazor14.Compile14, sourceCode: null),
        new("Custom.Hybrid.CodeTyped", CodeFileTypes.V16, HelpForRazorTyped.Compile16, sourceCode: null),
    ];

    /// <summary>
    /// Override ToString for better debugging
    /// </summary>
    public override string ToString() => _toString ??= $"{nameof(CodeFileInfo)} - {nameof(RelativePath)}: '{RelativePath}'; {nameof(FullPath)}: '{FullPath}'; {nameof(SourceCode)}: {SourceCode?.Length}; {nameof(Inherits)}: '{Inherits}'; {nameof(Type)}: '{Type}'; {nameof(AppCode)}: '{AppCode}'";
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
        { "AppCode", AppCode.ToString() },
    };

}