using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Code.Internal.Generate;

internal abstract class GeneratePropertyBase
{
    public abstract ValueTypes ForDataType { get; }

    protected GenerateCodeHelper CodeHelper => _codeHelper ??= new();
    private GenerateCodeHelper _codeHelper;

    public abstract List<GenCodeSnippet> Generate(IContentTypeAttribute attribute, int tabs);

    protected GenCodeSnippet GenPropSnip(int tabs, string returnType, string name, string method,
        NoParamOrder protector = default, string[] summary = default, string parameters = default, bool priority = true, List<string> usings = default)
    {
        var comment = CodeHelper.XmlComment(tabs, summary);
        return new(name, comment + GenProp(tabs, returnType, name, method, parameters: parameters), priority: priority, usings: usings);
    }

    private string GenProp(int tabs, string returnType, string name, string method, string parameters)
    {
        if (parameters.HasValue())
            parameters = ", " + parameters;

        return $"{CodeHelper.Indentation(tabs)}public {returnType} {name} => {method}(\"{name}\"{parameters});";
    }
}