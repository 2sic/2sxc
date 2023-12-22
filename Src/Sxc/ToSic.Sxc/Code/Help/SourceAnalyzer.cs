using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Code.Help;

// TODO: stv, analyzer works with razor files, and need enhancement for CS... 
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class SourceAnalyzer : ServiceBase
{
    private readonly IServerPaths _serverPaths;

    public SourceAnalyzer(IServerPaths serverPaths) : base("Sxc.RzrSrc")
    {
        ConnectServices(
            _serverPaths = serverPaths
        );
    }

    public CodeFileInfo TypeOfVirtualPath(string virtualPath)
    {
        var l = Log.Fn<CodeFileInfo>($"{nameof(virtualPath)}: '{virtualPath}'");
        try
        {
            var contents = GetFileContentsOfVirtualPath(virtualPath);
            return contents == null
                ? l.ReturnAndLog(CodeFileInfo.CodeFileNotFound)
                : l.ReturnAndLog(AnalyzeContent(virtualPath, contents));
        }
        catch
        {
            return l.ReturnAndLog(CodeFileInfo.CodeFileUnknown, "error trying to find type");
        }
    }

    private string GetFileContentsOfVirtualPath(string virtualPath)
    {
        var l = Log.Fn<string>($"{nameof(virtualPath)}: '{virtualPath}'");

        if (virtualPath.IsEmptyOrWs())
            return l.Return(null, "no path");

        var path = _serverPaths.FullContentPath(virtualPath);
        if (path == null || path.IsEmptyOrWs())
            return l.Return(null, "no path");

        if (!File.Exists(path))
            return l.Return(null, "file not found");

        var contents = File.ReadAllText(path);
        return l.Return(contents, $"found, {contents.Length} bytes");
    }

    public CodeFileInfo AnalyzeContent(string path, string contents)
    {
        var l = Log.Fn<CodeFileInfo>($"{nameof(path)}:{path}");
        if (contents.Length < 10)
            return l.Return(CodeFileInfo.CodeFileUnknown, "file too short");

        var isCs = path.ToLowerInvariant().EndsWith(ToSic.Sxc.Code.CodeCompiler.CsFileExtension);
        l.A($"isCs: {isCs}");

        if (isCs)
        {
            var csHasThisAppCode = IsThisAppCodeUsedInCs(contents);
            l.A($"thisApp: {csHasThisAppCode}");
            return l.Return(csHasThisAppCode ? CodeFileInfo.CodeFileUnknownWithThisAppCode : CodeFileInfo.CodeFileUnknown, "Ok, cs file");
        }

        // Cshtml part
        var inheritsMatch = Regex.Match(contents, @"@inherits\s+(?<BaseName>[\w\.]+)", RegexOptions.Multiline);

        if (!inheritsMatch.Success)
            return l.Return(CodeFileInfo.CodeFileUnknown, "no inherits found");

        var ns = inheritsMatch.Groups["BaseName"].Value;
        if (ns.IsEmptyOrWs())
            return l.Return(CodeFileInfo.CodeFileUnknown);

        var cshtmlHasThisAppCode = IsThisAppCodeUsedInCshtml(contents);

        var findMatch = CodeFileInfo.CodeFileList
            .FirstOrDefault(cf => cf.Inherits.EqualsInsensitive(ns) && cf.ThisApp == cshtmlHasThisAppCode);

        return findMatch != null
            ? l.ReturnAndLog(findMatch)
            : l.Return(CodeFileInfo.CodeFileOther, $"namespace '{ns}' can't be found");
    }

    private static bool IsThisAppCodeUsedInCshtml(string sourceCode)
    {
        // Pattern to match '@using ThisApp.Code' not commented out

        // TODO: stv, update code because this code is not robust enough
        // it does not handle all edge cases, event it does not work correctly in some cases

        const string pattern = @"
            # Ignore leading whitespaces
            (?<=^\s*)

            # Match the @using statement
            @using\s+ThisApp\.Code

            # Ensure that it's not part of a comment
            (?<!@(/\*)[\s\S]*?@using\s+ThisApp\.Code) # Not in Razor comment";

        var options = RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace;
        var thisAppMatch = Regex.Match(sourceCode, pattern, options);

        return thisAppMatch.Success;
    }

    private static bool IsThisAppCodeUsedInCs(string sourceCode)
    {
        // Pattern to match 'using ThisApp.Code;' not in single-line or multi-line comments

        // TODO: stv, update code because this code is not robust enough
        // it does not handle all edge cases, event it does not work correctly in some cases
        const string pattern = @"
            # Ignore leading whitespaces
            (?<=^\s*)

            # Match the 'using ThisApp.Code;' statement
            using\s+ThisApp\.Code\s*;

            # Ensure that it's not part of a single-line comment
            (?<!//.*using\s+ThisApp\.Code\s*;)

            # Ensure that it's not part of a multi-line comment
            (?<!/\*[\s\S]*?using\s+ThisApp\.Code\s*;)";

        var options = RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace;
        var thisAppMatch = Regex.Match(sourceCode, pattern, options);

        return thisAppMatch.Success;
    }

}