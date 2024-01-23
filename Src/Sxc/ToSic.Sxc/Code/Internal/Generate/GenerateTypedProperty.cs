using System.Text;
using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.Code.Internal.Generate;

internal class GenerateTypedProperty
{
    public GeneratedCode Generate(GenerateCodeHelper GenHelper, IContentTypeAttribute attribute)
    {
        // TODO:
        // - figure out MethodName - eg. String(...)
        // - figure out fallback value
        // - possible multi-properties eg. Link, LinkUrl, Image / Images
        // - add XML comment

        // String builder with empty line
        var sb = new StringBuilder();
        sb.AppendLine();

        var indent = GenHelper.Indentation(DataModelGenerator.DepthProperty);
        var type = ValueTypeHelpers.GetType(attribute.Type);
        if (type == null)
            return new( sb.AppendLine(indent + $"// Nothing generated for {attribute.Name} as type-specs missing"));

        sb.Append(GenHelper.XmlComment(indent, summary: $"todo - {attribute.Name}"));
        return new(sb.AppendLine($"{indent}public {type.Name} {attribute.Name} => {nameof(ICanBeItem.Item)}.{attribute.Type}();"));

    }
}