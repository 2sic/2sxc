using System.Text;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Code.Internal.Generate;

internal class CodeGenHelper
{
    internal const int Indent = 2;

    public string Indentation(int depth) => new(' ', Indent * depth);

    public void AddLines(StringBuilder sb, int lines)
    {
        for (var i = 0; i < lines; i++) sb.AppendLine();
    }

    public string CodeComment(int tabs, string comment, int padBefore = 1, int padAfter = default, int altGap = 1) 
        => CodeComment(tabs, comment.SplitNewLine(), padBefore, padAfter, altGap);

    public string CodeComment(int tabs, string[] comment, int padBefore = 1, int padAfter = default, int altGap = 1)
    {
        // If nothing, return empty lines as much as altGap
        if (!comment.SafeAny())
            return new('\n', altGap);

        // Summary
        var sb = new StringBuilder();
        AddLines(sb, padBefore);
        var indent = Indentation(tabs);
        foreach (var l in comment) sb.AppendLine($"{indent}// {l}");
        AddLines(sb, padAfter);

        return sb.ToString();
    }

    public string XmlComment(int tabs, string summary = default, int padBefore = 1, int padAfter = default, int altGap = 1)
        => XmlComment(tabs, summary.SplitNewLine(), padBefore, padAfter, altGap);

    public string XmlComment(int tabs, string[] summary = default, int padBefore = 1, int padAfter = default, int altGap = 1)
    {
        // If nothing, return empty lines as much as altGap
        if (summary.SafeNone() || (summary.Length == 1 && summary[0].IsEmptyOrWs()))
            return new('\n', altGap);

        // Summary
        var sb = new StringBuilder();
        var indent = Indentation(tabs);
        AddLines(sb, padBefore);

        if (summary.Length == 1)
        {
            sb.AppendLine($"{indent}/// <summary>{summary[0]}</summary>");
            AddLines(sb, padAfter);
            return sb.ToString();
        }

        sb.AppendLine($"{indent}/// <summary>");
        foreach (var l in summary) sb.AppendLine($"{indent}/// {l}");
        sb.AppendLine($"{indent}/// </summary>");
        AddLines(sb, padAfter);

        return sb.ToString();
    }

    public string GenerateUsings(List<string> usings)
    {
        if (usings == null || !usings.Any()) return null;
        var sb = new StringBuilder();
        foreach (var u in usings) sb.AppendLine($"using {u};");
        sb.AppendLine();
        return sb.ToString();
    }
}