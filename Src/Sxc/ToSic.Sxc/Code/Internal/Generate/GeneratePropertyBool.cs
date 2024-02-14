using System.Text;

namespace ToSic.Sxc.Code.Internal.Generate;

/// <summary>
/// Empty properties don't result in any code
/// </summary>
internal class GeneratePropertyBool: GeneratePropertyBase
{
    public override ValueTypes ForDataType => ValueTypes.Boolean;

    public override GeneratedCode Generate(GenerateCodeHelper genHelper, IContentTypeAttribute attribute, int indent)
    {
        var indentStr = genHelper.Indentation(indent);
        return new(new StringBuilder().AppendLine(GenerateProperty(indentStr, "bool", attribute.Name, "Bool")));
    }
}