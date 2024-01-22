using System.Text;

namespace ToSic.Sxc.Code.Internal.Generate;

internal class GenerateCodeHelper
{
    internal const int Indent = 4;

    public string Indentation(int depth) => new(' ', Indent * depth);

    public StringBuilder XmlComment(string indent, NoParamOrder protector = default, string summary = default)
    {
        var sb = new StringBuilder();

        // Summary
        if (summary != null)
        {
            sb.AppendLine($"{indent}/// <summary>");
            sb.AppendLine($"{indent}/// {summary}");
            sb.AppendLine($"{indent}/// </summary>");
        }

        return sb;
    }

}