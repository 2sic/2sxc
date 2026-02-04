using ToSic.Sxc.Code.Generate.Sys;

namespace ToSic.Sxc.Code.Generate.Data;

internal class CSharpTypedDataModelGenerator(CSharpTypedDataModelsGenerator generator, IContentType type, string baseName) 
    : CSharpModelGeneratorBase(generator, type, baseName, generator.Log, "Gen.DtaCls")
{
    #region Overrides

    protected override string ClassSuffix => Specs.Suffix ?? "";

    protected override string GenerateFileIntroComment(string userName) =>
        $$"""
          // DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
          // See also: https://go.2sxc.org/copilot-data
          // To extend it, create a "{{ClassName}}.cs" with this contents:
          /*
          namespace {{Generator.Specs.DataNamespace}}
          {
            public partial class {{ClassName}}
            {
              // Add your own properties and methods here
            }
          }
          */

          {{CSharpGeneratorHelper.GeneratorHeader(Generator, Specs, userName)}}
          
          """;

    protected override string GenerateMainClassComment(string? firstPropertyName)
    {
        var remarks = GenerateCommonScopeRemarks();
        return CodeGenHelper.CodeComment(Specs.TabsClass,
                   $"""
                    This is a generated class for {ClassName} {GetScopeDescription()}
                    To extend/modify it, see instructions above.
                    """)
               + CodeGenHelper.XmlComment(Specs.TabsClass, summary:
                   $"""
                    {ClassName} data. <br/>
                    Re-generate whenever you change the ContentType. <br/>
                    <br/>
                    Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
                    Most properties have a simple access, such as `.{firstPropertyName}`. <br/>
                    For other properties or uses, use methods such as
                    .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
                    """,
                   remarks: remarks);
    }

    protected override string GenerateAutoGenClassComment() =>
        CodeGenHelper.XmlComment(Specs.TabsClass,
            summary:
            $"Auto-Generated *base* class for '{ClassName}' in scope '{Type.Scope}'. " +
            $"It uses special names to avoid accidental use.");

    #endregion

}
