namespace ToSic.Sxc.Code.Internal.Generate;

internal abstract class GeneratePropertyBase
{
    public abstract ValueTypes ForDataType { get; }

    protected GenerateCodeHelper CodeHelper => _codeHelper ??= new();
    private GenerateCodeHelper _codeHelper;

    public abstract List<GenCodeSnippet> Generate(IContentTypeAttribute attribute, int tabs);

    protected string GenerateProperty(int tabs, string returnType, string name, string method)
        => $"{CodeHelper.Indentation(tabs)}public {returnType} {name} => {method}(\"{name}\");";

    protected string GenerateProperty(int tabs, string returnType, string name, string method, string parameters)
        => $"{CodeHelper.Indentation(tabs)}public {returnType} {name} => {method}(\"{name}\", {parameters});";
}