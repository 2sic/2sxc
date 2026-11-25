using ToSic.Eav.Apps;
using ToSic.Sxc.Code.Generate.Sys;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Code.Generate.Data;

// IMPORTANT: DON'T RENAME THIS CLASS
// The exact name is stored in configurations

/// <summary>
/// Experimental
/// Generator for C# Custom Model classes that inherit from CustomModel instead of CustomItem.
/// These are lightweight models without the full ITypedItem API surface.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class CSharpCustomModelsGenerator(IUser user, IAppReaderFactory appReadFac)
    : ServiceBase(SxcLogName + ".CMdGen"), IFileGenerator
{
    #region Information for the interface

    public FileGeneratorSpecs GeneratorSpecs => field ??= new(this)
    {
        Description = "Generates light-weight C# Custom Model Classes for the AppCode/Data folder",
        DescriptionHtml = $"The {GetType().Name} will generate <code>[TypeName]Model.Generated.cs</code> files in the <code>AppCode/Data</code> folder.",
        OutputLanguage = "CSharp",
        OutputType = "DataModel"
    };

    #endregion

    public IGeneratedFileSet[] Generate(GenerateFileSetSpecs specs)
    {
        var appReader = appReadFac.Get(specs.AppId);
        var buildSpecs = new CSharpCodeSpecs
        {
            AppName = appReader.Specs.Name,
            DataInherits = "Custom.Data.CustomModel", // Inherit from CustomModel instead of CustomItem
            FileGeneratedSuffix = ".Generated", // Suffix is handled in class name
            ExportedContentTypes = CSharpModelSpecsContentTypeHelper.GetSelectedContentTypes(appReader, specs.ContentTypes),
            SetSpecs = specs,
        };

        var fragmentGenerator = new CSharpCustomModelGenerator(GeneratorSpecs, Log);
        return new CSharpDataFileGenerator(Log).Generate(GeneratorSpecs, buildSpecs, user, "C# Custom Model Classes", fragmentGenerator);
    }

}
