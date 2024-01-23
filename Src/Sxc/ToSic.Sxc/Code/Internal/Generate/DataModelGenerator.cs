using System.Text;
using ToSic.Eav.Apps;
using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.Code.Internal.Generate;

/// <summary>
/// Experimental
/// </summary>
internal class DataModelGenerator
{
    internal const int DepthNamespace = 0;
    internal const int DepthClass = 1;
    internal const int DepthProperty = 2;
    internal const int Indent = 4;
    internal const string NamespaceBody = "[NAMESPACE-BODY]";
    internal const string ClassBody = "[CLASS-BODY]";

    internal GenerateCodeHelper GenHelper = new();

    public string Generate(IAppState state)
    {
        // TODO:
        // - add comment with date/time stamp
        // - add comment with version of generator
        // - add attribute to mention what data-type it's for, in case the name doesn't match to help with errors
        // - consider modifying the ToString to better show what it's doing
        // - check Equals of the new objects
        var sb = new StringBuilder();
        // Generate usings
        sb.Append(GenerateUsings());
        // Generate namespace
        sb.Append(GenerateNamespace("ThisApp.Data"));

        // Generate classes for all types in scope Default
        var types = state.ContentTypes.OfScope(Scopes.Default);
        var classesSb = new StringBuilder();
        foreach (var type in types)
        {
            var classSb = GenerateClass(type.Name);
            var propsSb = new StringBuilder();
            foreach (var attribute in type.Attributes) 
                propsSb.Append(GenerateProperty(attribute));
            classSb.Replace(ClassBody, propsSb.ToString());
            classesSb.Append(classSb);
        }

        sb.Replace(NamespaceBody, classesSb.ToString());

        return sb.ToString();
    }

    public StringBuilder GenerateUsings()
    {
        var sb = new StringBuilder();
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using System.Linq;");
        sb.AppendLine("using ThisApp.Data;");
        return sb;
    }

    public StringBuilder GenerateNamespace(string @namespace)
    {
        // TODO:
        // - configure initial namespace
        var sb = new StringBuilder();
        sb.AppendLine($"namespace {@namespace}");
        sb.AppendLine("{");
        sb.AppendLine(NamespaceBody);
        sb.AppendLine("}");
        return sb;
    }

    public StringBuilder GenerateClass(string className)
    {
        // TODO:
        // - base class
        // - additional base class when a property has the same name as the class
        var indent = GenHelper.Indentation(DepthClass);
        var sb = new StringBuilder();
        sb.AppendLine(indent + $"public partial class {className}");
        sb.AppendLine(indent + "{");
        // empty constructor with Xml Comment
        sb.Append(GenHelper.XmlComment(indent, summary: $"todo - empty constructor so As...<{className}>() works."));
        sb.AppendLine(GenHelper.Indentation(DepthProperty) + $"public {className}() {{ }}");

        // body
        sb.AppendLine(ClassBody);

        // close class
        sb.AppendLine(indent + "}");
        return sb;
    }

    public StringBuilder GenerateProperty(IContentTypeAttribute attribute)
    {
        // TODO:
        // - figure out MethodName - eg. String(...)
        // - figure out fallback value
        // - possible multi-properties eg. Link, LinkUrl, Image / Images
        // - add XML comment

        // String builder with empty line
        var sb = new StringBuilder();
        sb.AppendLine();

        var indent = GenHelper.Indentation(DepthProperty);
        var type = ValueTypeHelpers.GetType(attribute.Type);
        if (type == null)
            return sb.AppendLine(indent + $"// Nothing generated for {attribute.Name} as type-specs missing");

        sb.Append(GenHelper.XmlComment(indent, summary: $"todo - {attribute.Name}"));
        return sb.AppendLine($"{indent}public {type.Name} {attribute.Name} => {nameof(ICanBeItem.Item)}.{attribute.Type}();");
    }

}