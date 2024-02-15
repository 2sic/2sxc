//using System.Text;
//using ToSic.Sxc.Data.Internal;

//namespace ToSic.Sxc.Code.Internal.Generate;

//internal class GenerateTypedProperty: GeneratePropertyBase
//{
//    public override ValueTypes ForDataType => throw new NotImplementedException();

//    public override List<GenCodeSnippet> Generate(IContentTypeAttribute attribute, int tabs)
//    {
//        // TODO:
//        // - figure out MethodName - eg. String(...)
//        // - figure out fallback value
//        // - possible multi-properties eg. Link, LinkUrl, Image / Images
//        // - add XML comment

//        // String builder with empty line
//        var sb = new StringBuilder();
//        sb.AppendLine();

//        var indentStr = CodeHelper.Indentation(tabs);
//        var type = ValueTypeHelpers.GetType(attribute.Type);
//        if (type == null)
//            return [new("todo", indentStr + $"// Nothing generated for {attribute.Name} as type-specs missing")];

//        sb.Append(CodeHelper.XmlComment(indentStr, summary: $"todo - {attribute.Name}"));
//        return [new("todo", $"{indentStr}public {type.Name} {attribute.Name} => {nameof(ICanBeItem.Item)}.{attribute.Type}();")];

//    }
//}