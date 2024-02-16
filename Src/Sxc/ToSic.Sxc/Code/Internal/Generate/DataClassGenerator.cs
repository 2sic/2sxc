using System.Text;
using ToSic.Eav;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Code.Internal.Generate;

internal class DataClassGenerator(DataModelGenerator dmg, IContentType type, string className): HelperBase(dmg.Log, "Gen.DtaCls")
{
    internal CodeGenSpecs Specs { get; } = dmg.Specs;

    #region Class

    internal CodeFileRaw PrepareFile()
    {
        var l = Log.Fn<CodeFileRaw>($"{nameof(className)}: {className}; {nameof(type)}: {type?.Name} ({type?.NameId})");

        if (type == null)
            return l.ReturnNull("No content type provided");

        // Generate main partial class
        var autoGenClassName = className + Specs.DataClassGeneratedSuffix;
        var mainClass = dmg.ClassWrapper(className, false, true, autoGenClassName);

        // Generate AutoGen class with properties
        var classAutoGen = dmg.ClassWrapper(autoGenClassName, true, false, Specs.DataInherits);
        var (_, propsSb, usings, firstProperty) = ClassProperties(type.Attributes.ToList());
        var classCode = classAutoGen.ToString(propsSb);


        var fullBody =
            MainClassComment(firstProperty)
            + mainClass
            + AutoGenClassComment()
            + classCode;

        var fileContents =
            dmg.CodeGenHelper.GenerateUsings(usings)
            + dmg.NamespaceWrapper(Specs.DataNamespace)
                .ToString(fullBody);

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
                Generators = GenDataProperties.Generators(new()).Where(p => p.ForDataType == a.Type).ToList()
            })
            .Where(a => a.Generators.Any())
            .SelectMany(set =>
            {
                return set.Generators
                    .SelectMany(p => p.Generate(Specs, set.Attribute, Specs.TabsProperty));
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
          // See also: https://go.2sxc.org/hotbuild-autogen
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

          // Generator:   {{dmg.GetType().Name}} v{{SharedAssemblyInfo.AssemblyVersion}}
          // App/Edition: {{Specs.AppName}}/{{Specs.Edition}}
          // User:        {{userName}}
          // When:        {{DateTime.Now:u}}
          
          """;

    public string MainClassComment(string firstPropertyName) =>
        dmg.CodeGenHelper.CodeComment(Specs.TabsClass,
            $"""
             This is a generated class for {className}
             To extend/modify it, see instructions above.
             """)
        + dmg.CodeGenHelper.XmlComment(Specs.TabsClass, summary:
            $"""
             {className} data. <br/>
             Generated {DateTime.Now:u}. Re-generate whenever you change the ContentType. <br/>
             <br/>
             Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
             Most properties have a simple access, such as `.{firstPropertyName}`. <br/>
             For other properties or uses, use methods such as
             .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
             """);

    public string AutoGenClassComment() => 
        dmg.CodeGenHelper.XmlComment(Specs.TabsClass, summary: $"Auto-Generated base class for {className}.");

    #endregion

}