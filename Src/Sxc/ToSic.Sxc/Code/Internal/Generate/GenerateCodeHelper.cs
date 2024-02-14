using System.Text;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Code.Internal.Generate;

internal class GenerateCodeHelper
{
    internal const int Indent = 2;

    public string Indentation(int depth) => new(' ', Indent * depth);

    public void AddLines(StringBuilder sb, int lines)
    {
        for (var i = 0; i < lines; i++) sb.AppendLine();
    }

    public StringBuilder CodeComment(string indent, string comment, int padBefore, int padAfter, int altGap)
    {
        var sb = new StringBuilder();

        // Summary
        if (comment.HasValue()) 
        {
            AddLines(sb, padBefore);
            comment
                .SplitNewLine()
                .ToList()
                .ForEach(l => sb.AppendLine($"{indent}// {l}"));
            AddLines(sb, padAfter);
        }
        else
        {
            AddLines(sb, altGap);
        }

        return sb;
    }

    public StringBuilder XmlComment(string indent, NoParamOrder protector = default, string summary = default)
    {
        var sb = new StringBuilder();

        // Summary
        if (summary.HasValue())
        {
            sb.AppendLine($"{indent}/// <summary>");
            sb.AppendLine($"{indent}/// {summary}");
            sb.AppendLine($"{indent}/// </summary>");
        }

        return sb;
    }

}