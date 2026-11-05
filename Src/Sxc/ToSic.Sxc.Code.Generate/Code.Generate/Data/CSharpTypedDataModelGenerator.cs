using ToSic.Sxc.Code.Generate.Sys;

namespace ToSic.Sxc.Code.Generate.Data;

internal class CSharpTypedDataModelGenerator(CSharpTypedDataModelsGenerator generator) : HelperBase(generator.Log, "Gen.DtaCls"), ICSharpCodeFragmentGenerator
{
    #region Overrides

    public string GenerateFileIntroComment(CSharpFileSpecs fileSpecs) =>
        $$"""
          // DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
          // See also: https://go.2sxc.org/copilot-data
          // To extend it, create a "{{fileSpecs.ClassName}}.cs" with this contents:
          /*
          namespace {{generator.Specs.DataNamespace}}
          {
            public partial class {{fileSpecs.ClassName}}
            {
              // Add your own properties and methods here
            }
          }
          */

          {{CSharpGeneratorHelper.GeneratorHeader(generator, fileSpecs.FileSetSpecs, fileSpecs.UserName)}}
          
          """;

    public string GenerateMainClassComment(CSharpFileSpecs fileSpecs, string? firstPropertyName)
    {
        var remarks = fileSpecs.Type.GenerateCommonScopeRemarks();
        return generator.CodeGenHelper.CodeComment(fileSpecs.FileSetSpecs.TabsClass,
                   $"""
                    This is a generated class for {fileSpecs.ClassName} {fileSpecs.Type.GetScopeDescription()}
                    To extend/modify it, see instructions above.
                    """)
               + generator.CodeGenHelper.XmlComment(fileSpecs.FileSetSpecs.TabsClass, summary:
                   $"""
                    {fileSpecs.ClassName} data. <br/>
                    Re-generate whenever you change the ContentType. <br/>
                    <br/>
                    Default properties such as `.Title` or `.Id` are provided in the base class. <br/>
                    Most properties have a simple access, such as `.{firstPropertyName}`. <br/>
                    For other properties or uses, use methods such as
                    .IsNotEmpty("FieldName"), .String("FieldName"), .Children(...), .Picture(...), .Html(...).
                    """,
                   remarks: remarks);
    }

    public string GenerateAutoGenClassComment(CSharpFileSpecs fileSpecs) =>
        generator.CodeGenHelper.XmlComment(fileSpecs.FileSetSpecs.TabsClass,
            summary:
            $"Auto-Generated *base* class for '{fileSpecs.ClassName}' in scope '{fileSpecs.Type.Scope}'. " +
            $"It uses special names to avoid accidental use.");

    #endregion

}
