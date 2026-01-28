using ToSic.Sxc.Code.Generate.Sys;

namespace ToSic.Sxc.Code.Generate.Data;

/// <summary>
/// Generator for individual CustomModel classes
/// Creates lightweight model classes that inherit from CustomModel
/// </summary>
internal class CSharpCustomModelGenerator(CSharpCustomModelsGenerator cmg, IContentType type, string baseName) 
    : CSharpModelGeneratorBase(cmg, type, baseName, cmg.Log, "Gen.CstMdl")
{
    #region Overrides

    protected override string ClassSuffix => Specs.Suffix ?? ""; // Specs.BaseClassSuffix; // "Model"

    protected override string GenerateFileIntroComment(string userName) =>
        $$"""
          // DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
          // See also: https://go.2sxc.org/copilot-data
          // To extend it, create a "{{ClassName}}.cs" with this contents:
          /*
          namespace AppCode.Data
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
                    This is a generated custom model class for {ClassName} {GetScopeDescription()}
                    To extend/modify it, see instructions above.
                    """)
               + CodeGenHelper.XmlComment(Specs.TabsClass, summary:
                   $"""
                    {ClassName} custom model. <br/>
                    Re-generate whenever you change the ContentType. <br/>
                    <br/>
                    This is a lightweight model that inherits from CustomModel. <br/>
                    For properties, use the strongly-typed access such as `.{firstPropertyName}`. <br/>
                    For advanced features, consider using the full CustomItem instead.
                    """,
                   remarks: remarks);
    }

    protected override string GenerateAutoGenClassComment() =>
        CodeGenHelper.XmlComment(Specs.TabsClass,
            summary: $"Auto-Generated *base* class for '{ClassName}' in data scope '{Type.Scope}'. " +
                     $"It uses special names to avoid accidental use.");

    #endregion
}
