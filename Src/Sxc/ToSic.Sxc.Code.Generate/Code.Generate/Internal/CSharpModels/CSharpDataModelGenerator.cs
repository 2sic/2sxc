using System.Text;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Code.Generate.Internal;

internal class CSharpDataModelGenerator(CSharpDataModelsGenerator dmg, IContentType type, string className) : HelperBase(dmg.Log, "Gen.DtaCls")
{
    internal CSharpCodeSpecs Specs { get; } = dmg.Specs;

    #region Class

    internal GeneratedDataModel PrepareFile()
    {
        var l = Log.Fn<GeneratedDataModel>($"{nameof(className)}: {className}; {nameof(type)}: {type?.Name} ({type?.NameId})");

        if (type == null)
            return l.ReturnNull("No content type provided");

        // Generate main partial class
        var autoGenClassName = $"{Specs.DataClassGeneratedPrefix}{className}{Specs.DataClassGeneratedSuffix}";
        var mainClass = dmg.CodeGenHelper.ClassWrapper(className, false, true, Specs.NamespaceAutoGen + "." + autoGenClassName);

        // Generate AutoGen class with properties
        var classAutoGen = dmg.CodeGenHelper.ClassWrapper(autoGenClassName, true, false, Specs.DataInherits);
        var (_, propsSb, usings, firstProperty) = ClassProperties(type.Attributes.ToList());
        var classCode = classAutoGen.ToString(propsSb);

        var autoGenClass =
            AutoGenClassComment()
            + classCode;

        var fullBody =
            MainClassComment(firstProperty)
            + mainClass;
        // + autoGenClass;

        var fileContents =
            dmg.CodeGenHelper.GenerateUsings(usings)
            + dmg.CodeGenHelper.NamespaceWrapper(Specs.DataNamespace)
                .ToString(fullBody)
            + "\n\n"
            + dmg.CodeGenHelper.NamespaceWrapper(Specs.DataNamespaceGenerated)
                .ToString(autoGenClass);

        return new(className + Specs.FileGeneratedSuffix, fileContents, FileIntroComment(dmg.User.Name));
    }

    #endregion

    #region Properties

    private (bool HasProps, string Code, List<string> Usings, string FirstProperty) ClassProperties(List<IContentTypeAttribute> attributes)
    {
        var l = Log.Fn<(bool, string, List<string>, string)>($"{nameof(attributes)}: {attributes.Count}");

        // Generate all properties with the helpers
        var propsSnippets = attributes
            .Select(a => new
            {
                Attribute = a,
                Generators = GenDataProperties.Generators(dmg.CodeGenHelper).Where(p => p.ForDataType == a.Type).ToList()
            })
            .Where(a => a.Generators.Any())
            .SelectMany(set =>
            {
                return set.Generators
                    .SelectMany(p => p.Generate(set.Attribute, Specs.TabsProperty));
            })
            .ToList();

        if (!propsSnippets.Any())
            return l.Return((false, null, null, null));

        // Detect duplicate names as this would fail
        // If we have duplicates, keep the first with a real priority
        var deduplicated = propsSnippets
            .GroupBy(ps => ps.NameId)
            .SelectMany(g => g.OrderBy(ps => ps.Priority ? 0 : 1).Take(1))
            .OrderBy(ps => ps.NameId)
            .ToList();

        var sb = new StringBuilder();
        foreach (var genCode in deduplicated)
            sb.AppendLine(genCode.ToString());

        var usings = deduplicated.SelectMany(ps => ps.Usings)
            .Distinct()
            .OrderBy(u => u)
            .ToList();

        return l.Return((true, sb.ToString(), usings, deduplicated.First().NameId));
    }


    #endregion

    #region Comments

    public string FileIntroComment(string userName) =>
        $$"""
          // DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
          // See also: https://go.2sxc.org/copilot-data
          // To extend it, create a "{{className}}.cs" with this contents:
          /*
          namespace AppCode.Data
          {
            public partial class {{className}}
            {
              // Add your own properties and methods here
            }
          }
          */

          {{CSharpGeneratorHelper.GeneratorHeader(dmg, Specs, userName)}}
          
          """;


    public string MainClassComment(string firstPropertyName)
    {
        var scope = type.Scope;
        var scopeIsSpecial = scope != Scopes.Default;
        var remarks = scopeIsSpecial ? $"This Content-Type is NOT in the default scope, so you may not see it in the Admin UI. It's in the scope {scope}." : null;
        return dmg.CodeGenHelper.CodeComment(Specs.TabsClass,
                   $"""
                    This is a generated class for {className} {(scopeIsSpecial ? $"(scope: {scope})" : "")}
                    To extend/modify it, see instructions above.
                    """)
               + dmg.CodeGenHelper.XmlComment(Specs.TabsClass, summary:
                   $"""
                    {className} data. <br/>
                    Re-generate whenever you change the ContentType. <br/>
                    <br/>
                    Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
                    Most properties have a simple access, such as `.{firstPropertyName}`. <br/>
                    For other properties or uses, use methods such as
                    .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
                    """,
                   remarks: remarks);
    }

    public string AutoGenClassComment()
    {
        return dmg.CodeGenHelper.XmlComment(Specs.TabsClass,
            summary:
            $"Auto-Generated base class for {type.Scope}.{className} in separate namespace and special name to avoid accidental use.");
    }

    #endregion

}