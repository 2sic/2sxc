using ToSic.Eav.Apps;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Code.Generate.Sys.CSharpBaseClasses;

/// <summary>
/// Experimental
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class WebApiGenerator(IUser user, IAppReaderFactory appReadFac)
    : ServiceBase($"{SxcLogName}.WebApiGen"), IFileGenerator
{
    #region Information for the interface

    public FileGeneratorSpecs GeneratorSpecs => field ??= new(this)
    {
        OutputLanguage = "CSharp",
        OutputType = "WebApi",
        Description = "Generates CSharp WebApi Base Controller in the AppCode/Api folder (WIP)",
        DescriptionHtml = $"The {GetType().Name} will generate <code>[Prefix]ControllerBase[Suffix].Generated.cs</code> files in the <code>AppCode/Api</code> folder. <br> IMPORTANT: Requires that Content-Types were generated."
    };

    #endregion


    public new IGeneratedFileSet[] Generate(IGenerateFileSetSpecs specs)
    {
        var cSharpSpecs = CSharpGeneratorShared.BuildSpecs(appReadFac.Get(specs.AppId).Specs.Name, specs);

        var file = CSharpServicesGenerator.GenerateCSharpService(GeneratorSpecs, cSharpSpecs, user, "ControllerBase", "Custom.Hybrid.ApiTyped", "Api", Log);

        var result = GeneratedFileSet.FromSpecs(GeneratorSpecs, "C# WebApi", [file]);
        return [result];
    }

}
