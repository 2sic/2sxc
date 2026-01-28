using ToSic.Sxc.Code.Generate.Sys;

namespace ToSic.Sxc.Code.Generate.Data;

internal class CSharpTypedDataModelGenerator(CSharpTypedDataModelsGenerator dmg, IContentType type, string baseName) 
    : CSharpModelGeneratorBase(dmg, type, baseName, dmg.Log, "Gen.DtaCls")
{
    #region Overrides

    protected override string ClassSuffix => Specs.Suffix ?? "";

    protected override string GenerateFileIntroComment(string userName) =>
        $$"""
          // DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
          // See also: https://go.2sxc.org/copilot-data
          // To extend it, create a "{{Prefix}}{{BaseName}}{{Suffix}}.cs" with this contents:
          /*
          namespace {{dmg.Specs.DataNamespace}}
          {
            public partial class {{Prefix}}{{BaseName}}{{Suffix}}
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
                    This is a generated class for {Prefix}{BaseName}{Suffix} {GetScopeDescription()}
                    To extend/modify it, see instructions above.
                    """)
               + CodeGenHelper.XmlComment(Specs.TabsClass, summary:
                   $"""
                    {Prefix}{BaseName}{Suffix} data. <br/>
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
            $"Auto-Generated *base* class for '{Prefix}{BaseName}{Suffix}' in scope '{Type.Scope}'. " +
            $"It uses special names to avoid accidental use.");

    #endregion

}
