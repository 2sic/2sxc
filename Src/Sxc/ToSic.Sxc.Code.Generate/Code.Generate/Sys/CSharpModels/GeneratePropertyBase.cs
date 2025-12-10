namespace ToSic.Sxc.Code.Generate.Sys;

internal abstract class GeneratePropertyBase(CSharpGeneratorHelper helper)
{
    protected CSharpCodeSpecs Specs = helper.Specs;

    public abstract ValueTypes ForDataType { get; }

    public abstract List<CodeFragment> Generate(IContentTypeAttribute attribute, int tabs);

    protected CodeFragment GenPropSnip(int tabs, string returnType, string name, string method,
        NoParamOrder npo = default,
        string? sourceName = default,
        string[]? summary = default,
        string[]? remarks = default,
        string[]? returns = default,
        string? parameters = default,
        bool priority = true,
        List<string>? usings = default,
        bool cache = false,
        bool jsonIgnore = false)
    {
        var overrideProp = helper.OverridePropertyNames.Contains(name);
        var overrideMeth = helper.OverrideMethods.Contains(method);
        var isOverride = overrideProp || overrideMeth;
        if (isOverride)
        {
            var originalName = overrideProp ? $"property {name}" : $"method {name}(...)";
            remarks =
            [
                ..remarks ?? [],
                $"This hides base {originalName}.",
                "To access original, convert using AsItem(...) or cast to ITypedItem.",
                "Consider renaming this field in the underlying content-type."
            ];
        }

        var comment = helper.XmlComment(tabs, summary: summary, remarks: remarks, returns: returns);
        return new(
            name,
            comment
            + GenAttribute(jsonIgnore, tabs)
            + GenProp(tabs, returnType, name, sourceName ?? name, method, parameters: parameters, cache, isOverride),
            priority: priority,
            usings: GenUsings(usings ?? [], jsonIgnore)
        );
    }

    private string GenAttribute(bool jsonIgnore, int tabs) 
        => jsonIgnore ? $"\n{helper.Indent(tabs)}[JsonIgnore]\n" : "";

    private string GenProp(int tabs, string returnType, string name, string sourceName, string method, string? parameters, bool cache, bool isOverride)
    {
        if (parameters.HasValue())
            parameters = ", " + parameters;

        var indent = helper.Indent(tabs);

        var cacheVarName = $"_{char.ToLower(name[0])}{name.Substring(1)}";
        var cacheResult = cache ? $"{cacheVarName} ??= " : "";

        var newPrefix = isOverride ? "new " : "";

        var mainCode = $"{indent}public {newPrefix}{returnType} {name} => {cacheResult}{method}(\"{sourceName}\"{parameters});";

        var cacheCode = cache
            ? $"\n{indent}private {returnType} {cacheVarName};"
            : null;

        return mainCode + cacheCode;
    }

    private static List<string> GenUsings(List<string> usings, bool jsonIgnore) 
        => jsonIgnore 
            ? ["System.Text.Json.Serialization", .. usings] 
            : usings;




}