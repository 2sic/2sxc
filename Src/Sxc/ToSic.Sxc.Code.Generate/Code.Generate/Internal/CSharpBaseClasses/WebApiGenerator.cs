using ToSic.Eav.Apps;
using ToSic.Eav.Context;

namespace ToSic.Sxc.Code.Generate.Internal.CSharpBaseClasses;

/// <summary>
/// Experimental
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class WebApiGenerator(IUser user, IAppReaderFactory appReadFac)
    : CSharpServicesGenerator(user, appReadFac), IFileGenerator
{
    #region Information for the interface

    public new string Description => "Generates CSharp WebApi Base Controller in the AppCode/Api folder (WIP)";

    public new string DescriptionHtml => $"The {Name} will generate <code>ControllerBase.Generated.cs</code> files in the <code>AppCode/Api</code> folder. <br> IMPORTANT: Requires that Content-Types were generated.";

    public new string OutputType => "WebApi";

    #endregion


    public new IGeneratedFileSet[] Generate(IFileGeneratorSpecs specs)
    {
        var cSharpSpecs = BuildSpecs(specs);

        var file = CSharpServiceBase(cSharpSpecs, "ControllerBase", "Custom.Hybrid.ApiTyped", "Api");

        var result = new GeneratedFileSet
        {
            Name = "C# Services",
            Description = Description,
            Generator = $"{Name} v{Version}",
            Path = GenerateConstants.PathToAppCode,
            Files = [file],
        };
        return [result];
    }

}