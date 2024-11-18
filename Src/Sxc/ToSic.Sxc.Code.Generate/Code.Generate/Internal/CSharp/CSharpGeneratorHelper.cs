using System.Text;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Code.Generate.Internal;

internal class CSharpGeneratorHelper(CSharpCodeSpecs specs)
{
    public CSharpCodeSpecs Specs => specs;

    public string Indent(int depth) => new(' ', specs.TabSize * depth);

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
        var indent = Indent(tabs);
        foreach (var l in comment) sb.AppendLine($"{indent}// {l}");
        AddLines(sb, padAfter);

        return sb.ToString();
    }

    public string XmlComment(int tabs, string summary = default, string remarks = default, string returns = default, int padBefore = 1, int padAfter = default,
        int altGap = 1)
        => XmlComment(tabs, summary: summary.SplitNewLine(), remarks: remarks.SplitNewLine(), returns: returns.SplitNewLine(), padBefore: padBefore, padAfter: padAfter, altGap: altGap);

    public string XmlComment(int tabs, string[] summary = default, string[] remarks = default, string[] returns = default, int padBefore = 1, int padAfter = default, int altGap = 1)
    {
        // 1. If nothing, return empty lines as much as altGap
        // first merge all the comments to see if we have any
        var merged = (summary ?? []).Concat(returns ?? []).ToList();
        if (!merged.Any() || merged.All(s => s.IsEmptyOrWs()))
            return new('\n', altGap);

        var sb = new StringBuilder();
        AddLines(sb, padBefore);
        var indent = Indent(tabs);

        // Summary
        var summaryComment = XmlCommentOne(indent, "summary", summary);
        if (summaryComment.HasValue()) sb.Append(summaryComment);

        // Remarks
        var remarksComment = XmlCommentOne(indent, "remarks", remarks);
        if (remarksComment.HasValue()) sb.Append(remarksComment);

        // Returns
        var returnsComment = XmlCommentOne(indent, "returns", returns);
        if (returnsComment.HasValue()) sb.Append(returnsComment);

        AddLines(sb, padAfter);

        return sb.ToString();
    }

    private static string XmlCommentOne(string indent, string tagName, string[] comments = default)
    {
        // If nothing, return empty lines as much as altGap
        if (comments == null || comments.All(s => s.IsEmptyOrWs()))
            return null;

        // Single liner
        // 2024-02-16 2dm disabled for now, seems like an optimization but the code is harder to read
        //if (comments.Length == 1)
        //    return $"{indent}/// <{tagName}>{comments[0]}</{tagName}>";

        // Multi-liner
        var sb = new StringBuilder();
        sb.AppendLine($"{indent}/// <{tagName}>");
        foreach (var l in comments) sb.AppendLine($"{indent}/// {l}");
        sb.AppendLine($"{indent}/// </{tagName}>");

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

    internal CodeFragment NamespaceWrapper(string @namespace)
    {
        return new("namespace", $"{Indent(specs.TabsNamespace)}namespace {@namespace}" + "\n{", closing: "}");
    }

    internal CodeFragment ClassWrapper(string className, bool isAbstract, bool isPartial, string inherits)
    {
        var indent = Indent(specs.TabsClass);
        var specifiers = (isAbstract ? "abstract " : "") + (isPartial ? "partial " : "");
        inherits = inherits.NullOrGetWith(i => $": {i}");

        var start = $$"""
                      {{indent}}public {{specifiers}}class {{className}}{{inherits}}
                      {{indent}}{
                      """;
        return new("class", start, closing: $"{indent}}}\n");
    }

    public static string GeneratorHeader(IFileGenerator generator, CSharpCodeSpecs specs, string userName) =>
        $"""
         // Generator:   {generator.Name}
         // App/Edition: {specs.AppName}/{specs.Edition}
         // User:        {userName}
         """;

    public static string DoNotModifyMessage = """
                                              // DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
                                              // All the classes are partial, so you can extend them in a separate file.
                                              """;
}