using Custom.Data;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Code.Internal.Generate;

internal abstract class GeneratePropertyBase
{
    public const string ItemAccessor = nameof(Item16._myItem);

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

    private string GenProp(int tabs, string returnType, string name, string method, string parameters, bool cache = false)
    {
        if (parameters.HasValue())
            parameters = ", " + parameters;

        var indent = CodeGenHelper.Indent(tabs);

        var cacheVarName = $"_{char.ToLower(name[0])}{name.Substring(1)}";
        var cacheResult = cache ? $"{cacheVarName} ??= " : "";
        var mainCode = $"{indent}public {returnType} {name} => {cacheResult}{method}(\"{name}\"{parameters});";

        var cacheCode = cache
            ? $"\n{indent}private {returnType} {cacheVarName};"
            : null;

        return mainCode + cacheCode;
    }
}