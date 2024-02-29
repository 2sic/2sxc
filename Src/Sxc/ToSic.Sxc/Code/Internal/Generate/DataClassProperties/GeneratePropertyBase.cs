using ToSic.Eav.Plumbing;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Code.Internal.Generate;

internal abstract class GeneratePropertyBase
{
    public abstract ValueTypes ForDataType { get; }

    protected CodeGenHelper CodeGenHelper => _codeGenHelper ??= new(new());
    private CodeGenHelper _codeGenHelper;

    public abstract List<CodeFragment> Generate(CodeGenSpecs specs, IContentTypeAttribute attribute, int tabs);

    protected CodeFragment GenPropSnip(int tabs, string returnType, string name, string method,
        NoParamOrder protector = default,
        string[] summary = default,
        string[] remarks = default,
        string[] returns = default,
        string parameters = default, bool priority = true, List<string> usings = default, bool cache = false)
    {
        var comment = CodeGenHelper.XmlComment(tabs, summary: summary, remarks: remarks, returns: returns);
        return new(
            name,
            comment + GenProp(tabs, returnType, name, method, parameters: parameters, cache)
            , priority: priority,
            usings: usings
        );
    }

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
        nameof(ITypedItem.IsDemoItem)
    ];

    private string GenProp(int tabs, string returnType, string name, string method, string parameters, bool cache = false)
    {
        if (parameters.HasValue())
            parameters = ", " + parameters;

        var indent = CodeGenHelper.Indent(tabs);

        var cacheVarName = $"_{char.ToLower(name[0])}{name.Substring(1)}";
        var cacheResult = cache ? $"{cacheVarName} ??= " : "";

        var newPrefix = OverridePropertyNames.Contains(name) ? "new " : "";

        var mainCode = $"{indent}public {newPrefix}{returnType} {name} => {cacheResult}{method}(\"{name}\"{parameters});";

        var cacheCode = cache
            ? $"\n{indent}private {returnType} {cacheVarName};"
            : null;

        return mainCode + cacheCode;
    }
}