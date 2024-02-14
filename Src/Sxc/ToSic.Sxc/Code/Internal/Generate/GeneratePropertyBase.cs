namespace ToSic.Sxc.Code.Internal.Generate;

internal abstract class GeneratePropertyBase
{
    public abstract string ForDataType { get; }

    public abstract GeneratedCode Generate(GenerateCodeHelper genHelper, IContentTypeAttribute attribute, int indent);

    protected string GenerateProperty(string indent, string returnType, string name, string method)
        => $"{indent}public {returnType} {name} => {method}();";
    protected string GenerateProperty(string indent, string returnType, string name, string method, string parameters)
        => $"{indent}public {returnType} {name} => {method}({parameters});";
}