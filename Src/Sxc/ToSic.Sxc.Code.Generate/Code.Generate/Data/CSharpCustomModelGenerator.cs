using ToSic.Sxc.Code.Generate.Sys;

namespace ToSic.Sxc.Code.Generate.Data;

/// <summary>
/// Generator for individual CustomModel classes
/// Creates lightweight model classes that inherit from CustomModel
/// </summary>
internal class CSharpCustomModelGenerator(CSharpCustomModelsGenerator generator) : HelperBase(generator.Log, "Gen.CstMdl"), ICSharpCodeFragmentGenerator
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
                    This is a generated custom model class for {fileSpecs.ClassName} {fileSpecs.Type.GetScopeDescription()}
                    To extend/modify it, see instructions above.
                    """)
               + generator.CodeGenHelper.XmlComment(fileSpecs.FileSetSpecs.TabsClass, summary:
                   $"""
                    {fileSpecs.ClassName} custom model. <br/>
                    Re-generate whenever you change the ContentType. <br/>
                    <br/>
                    This is a lightweight model that inherits from CustomModel. <br/>
                    For properties, use the strongly-typed access such as `.{firstPropertyName}`. <br/>
                    For advanced features, consider using the full CustomItem instead.
                    """,
                   remarks: remarks);
    }

    public string GenerateAutoGenClassComment(CSharpFileSpecs fileSpecs) =>
        generator.CodeGenHelper.XmlComment(fileSpecs.FileSetSpecs.TabsClass,
            summary: $"Auto-Generated *base* class for '{fileSpecs.ClassName}' in data scope '{fileSpecs.Type.Scope}'. " +
                     $"It uses special names to avoid accidental use.");

    #endregion
}
