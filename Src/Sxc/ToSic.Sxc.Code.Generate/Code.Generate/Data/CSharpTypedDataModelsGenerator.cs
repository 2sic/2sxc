using ToSic.Eav.Apps;
using ToSic.Sxc.Code.Generate.Sys;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Code.Generate.Data;

// IMPORTANT: DON'T RENAME THIS CLASS
// The exact name is stored in configurations

/// <summary>
/// Experimental
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class CSharpTypedDataModelsGenerator(IUser user, IAppReaderFactory appReadFac)
    : ServiceBase(SxcLogName + ".DMoGen"), IFileGenerator
{
    #region Information for the interface

    public FileGeneratorSpecs GeneratorSpecs => field ??= new(this)
    {
        OutputLanguage = "CSharp",
        OutputType = "DataModel",
        Description = "Generates C# Data Classes for the AppCode/Data folder",
        DescriptionHtml = $"The {GetType().Name} will generate <code>[TypeName].Generated.cs</code> files in the <code>AppCode/Data</code> folder.",
    };

    #endregion


    public IGeneratedFileSet[] Generate(GenerateFileSetSpecs specs)
    {
        var appReader = appReadFac.Get(specs.AppId);
        var buildSpecs = new CSharpCodeSpecs
        {
            AppName = appReader.Specs.Name,
            OverrideMethods = ModelGeneratorConstants.OverrideMethods,
            OverrideProperties = ModelGeneratorConstants.OverridePropertyNames,
            ExportedContentTypes = CSharpModelSpecsContentTypeHelper.GetSelectedContentTypes(appReader, specs.ContentTypes),
            SetSpecs = specs,
        };

        var fragmentGenerator = new CSharpTypedDataModelGenerator(GeneratorSpecs, Log);
        return new CSharpDataFileGenerator(Log).Generate(GeneratorSpecs, buildSpecs, user, "C# Data Classes", fragmentGenerator);
    }

}
