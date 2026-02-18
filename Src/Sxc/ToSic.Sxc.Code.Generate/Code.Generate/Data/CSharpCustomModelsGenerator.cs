using ToSic.Eav.Apps;
using ToSic.Sxc.Code.Generate.Sys;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Code.Generate.Data;

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

    public override string Description => "Generates light-weight C# Custom Model Classes for the AppCode/Data folder";

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

        codeSpecs = codeSpecs with
        {
            //DataClassGeneratedSuffix = modelSuffix, // Custom suffix for model classes
            DataInherits = "Custom.Data.CustomModel", // Inherit from CustomModel instead of CustomItem
            FileGeneratedSuffix = ".Generated" // Suffix is handled in class name
        };
        return codeSpecs;
    }

    protected override IGeneratedFile? CreateFileGenerator(IContentType type, string baseName)
    {
        // Empty the list of override methods and property names
        // since the model-generator doesn't add any overrides
        // because the base class doesn't have any own methods or properties.
        CodeGenHelper.OverrideMethods = [];
        CodeGenHelper.OverridePropertyNames = [];
        return new CSharpCustomModelGenerator(this, type, baseName).PrepareFile();
    }
}
