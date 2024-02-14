using System.Text;
using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.Code.Internal.Generate;

internal class GenerateTypedProperty: GeneratePropertyBase
{
    public override string ForDataType => throw new NotImplementedException();

    public override GeneratedCode Generate(GenerateCodeHelper genHelper, IContentTypeAttribute attribute, int indent)
    {
        // TODO:
        // - figure out MethodName - eg. String(...)
        // - figure out fallback value
        // - possible multi-properties eg. Link, LinkUrl, Image / Images
        // - add XML comment

        // String builder with empty line
        var sb = new StringBuilder();
        sb.AppendLine();

        var indentStr = genHelper.Indentation(indent);
        var type = ValueTypeHelpers.GetType(attribute.Type);
        if (type == null)
            return new( sb.AppendLine(indentStr + $"// Nothing generated for {attribute.Name} as type-specs missing"));

        sb.Append(genHelper.XmlComment(indentStr, summary: $"todo - {attribute.Name}"));
        return new(sb.AppendLine($"{indentStr}public {type.Name} {attribute.Name} => {nameof(ICanBeItem.Item)}.{attribute.Type}();"));

    }
}