using ToSic.Eav.Apps;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Code.Generate.Sys;

/// <summary>
/// Experimental
/// Generator for C# Custom Model classes that inherit from CustomModel instead of CustomItem.
/// These are lightweight models without the full ITypedItem API surface.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class CSharpCustomModelsGenerator(IUser user, IAppReaderFactory appReadFac)
    : CSharpModelsGeneratorBase(user, appReadFac, SxcLogName + ".CMdGen") // IFileGenerator is inherited from base
{
    #region Information for the interface

    public override string Description => "Generates C# Custom Model Classes for the AppCode/Data folder";

    public override string DescriptionHtml => $"The {Name} will generate <code>[TypeName]Model.Generated.cs</code> files in the <code>AppCode/Data</code> folder.";

    protected override string GeneratedSetName => "C# Custom Model Classes";

    #endregion

    protected internal override CSharpCodeSpecs BuildDerivedSpecs(IFileGeneratorSpecs parameters)
        => BuildCustomModelSpecs(parameters);

    /// <summary>
    /// Build CustomModel-specific specs with CustomModel inheritance and naming conventions
    /// </summary>
    private CSharpCodeSpecs BuildCustomModelSpecs(IFileGeneratorSpecs parameters)
    {
        var codeSpecs = base.BuildSpecs(parameters);

        // Override defaults for CustomModel generation
        codeSpecs.DataClassGeneratedSuffix = "Model"; // Custom suffix for model classes
        codeSpecs.DataInherits = "Custom.Data.CustomModel"; // Inherit from CustomModel instead of CustomItem
        codeSpecs.FileGeneratedSuffix = "Model.Generated"; // Custom suffix for model files
        return codeSpecs;
    }

    protected override IGeneratedFile? CreateFileGenerator(IContentType type, string className)
    {
        // Empty the list of override methods and property names
        // since the model-generator doesn't add any overrides
        // because the base class doesn't have any own methods or properties.
        CodeGenHelper.OverrideMethods = [];
        CodeGenHelper.OverridePropertyNames = [];
        return new CSharpCustomModelGenerator(this, type, className).PrepareFile();
    }
}
