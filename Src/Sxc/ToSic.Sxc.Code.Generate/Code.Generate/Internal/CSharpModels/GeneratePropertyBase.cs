using ToSic.Eav.Plumbing;
using ToSic.Lib.Coding;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Code.Generate.Internal;

internal abstract class GeneratePropertyBase(CSharpGeneratorHelper helper)
{
    protected CSharpCodeSpecs Specs = helper.Specs;

    protected CSharpGeneratorHelper CodeGenHelper = helper;

    public abstract ValueTypes ForDataType { get; }

    public abstract List<CodeFragment> Generate(IContentTypeAttribute attribute, int tabs);

    protected CodeFragment GenPropSnip(int tabs, string returnType, string name, string method,
        NoParamOrder protector = default,
        string sourceName = default,
        string[] summary = default,
        string[] remarks = default,
        string[] returns = default,
        string parameters = default, bool priority = true, List<string> usings = default, bool cache = false, bool jsonIgnore = false)
    {
        var overrideProp = OverridePropertyNames.Contains(name);
        var overrideMeth = OverrideMethods.Contains(method);
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

        var comment = CodeGenHelper.XmlComment(tabs, summary: summary, remarks: remarks, returns: returns);
        return new(
            name,
            comment
            + GenAttribute(jsonIgnore, tabs)
            + GenProp(tabs, returnType, name, sourceName ?? name, method, parameters: parameters, cache, isOverride),
            priority: priority,
            usings: GenUsings(usings, jsonIgnore)
        );
    }

    private string GenAttribute(bool jsonIgnore, int tabs) 
        => jsonIgnore ? $"\n{CodeGenHelper.Indent(tabs)}[JsonIgnore]\n" : "";

    private string GenProp(int tabs, string returnType, string name, string sourceName, string method, string parameters, bool cache, bool isOverride)
    {
        if (parameters.HasValue())
            parameters = ", " + parameters;

        var indent = CodeGenHelper.Indent(tabs);

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

    /// <summary>
    /// These names exist in the base class, so we need to override them with `new`.
    /// It would work without this override, but would add warnings that can confuse users.
    /// </summary>
    private static readonly string[] OverridePropertyNames =
    [
        nameof(ITypedItem.Id),
        nameof(ITypedItem.Guid),
        nameof(ITypedItem.Title),
        nameof(ITypedItem.Type),
        nameof(ITypedItem.Metadata),
        nameof(ITypedItem.Presentation),
        nameof(ITypedItem.IsPublished),
        nameof(ITypedItem.Publishing),
        // nameof(ITypedItem.Dyn), - this one is explicitly implemented, so it's not available
        nameof(ITypedItem.IsDemoItem),
        nameof(ITypedItem.Entity),
        nameof(ITypedItem.Type),
    ];

    private static readonly string[] OverrideMethods =
    [
        nameof(ITypedItem.Field),

        nameof(ITypedItem.Get),
        nameof(ITypedItem.Bool),
        nameof(ITypedItem.DateTime),
        nameof(ITypedItem.String),
        nameof(ITypedItem.Int),
        nameof(ITypedItem.Double),
        nameof(ITypedItem.Decimal),
        nameof(ITypedItem.Long),
        nameof(ITypedItem.Float),

        nameof(ITypedItem.Child),
        nameof(ITypedItem.Children),
        nameof(ITypedItem.Parent),
        nameof(ITypedItem.Parents),

        nameof(ITypedItem.Attribute),
        nameof(ITypedItem.Url),
        nameof(ITypedItem.Img),
        nameof(ITypedItem.Picture),
        nameof(ITypedItem.Html),

        nameof(ITypedItem.File),
        nameof(ITypedItem.Folder),
        nameof(ITypedItem.Gps),

        // Key / value handling methods
        nameof(ITypedItem.ContainsKey),
        nameof(ITypedItem.IsEmpty),
        nameof(ITypedItem.IsNotEmpty),
        nameof(ITypedItem.Keys),
    ];

}