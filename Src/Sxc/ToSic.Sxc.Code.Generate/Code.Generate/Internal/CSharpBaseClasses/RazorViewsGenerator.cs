using ToSic.Eav.Apps;
using ToSic.Eav.Context;

namespace ToSic.Sxc.Code.Generate.Internal.CSharpBaseClasses;

/// <summary>
/// Experimental
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class RazorViewsGenerator(IUser user, IAppReaderFactory appReadFac)
    : CSharpGeneratorBase(user, appReadFac, SxcLogName + ".RzrVGn"), IFileGenerator
{
    #region Information for the interface

    public string Description => "Generates Razor Views in the AppCode/Razor folder";

    public string DescriptionHtml => $"The {Name} will generate <code>AppRazor.Generated.cs</code> files in the <code>AppCode/Razor</code> folder. <br> IMPORTANT: Requires that Content-Types were generated.";

    public string OutputType => "RazorView";

    #endregion


    public IGeneratedFileSet[] Generate(IFileGeneratorSpecs specs)
    {
        var cSharpSpecs = BuildSpecs(specs);
        var file = AppRazors(cSharpSpecs);

        var result = new GeneratedFileSet
        {
            Name = "C# Razor Views",
            Description = Description,
            Generator = $"{Name} v{Version}",
            Path = GenerateConstants.PathToAppCode,
            Files = [file],
        };
        return [result];
    }

    private GeneratedFile AppRazors(CSharpCodeSpecs cSharpSpecs)
    {
        var (_, appSnip, allUsings) = BaseClassHelper.BaseClassTools(cSharpSpecs);

        var file = new GeneratedFile
        {
            FileName = "AppRazor.Generated.cs",
            Path = "Razor",
            Body = $$"""
                     {{CSharpGeneratorHelper.DoNotModifyMessage}}

                     {{CSharpGeneratorHelper.GeneratorHeader(this, cSharpSpecs, User.Name)}}

                     {{string.Join("\n", allUsings)}}

                     namespace AppCode.Razor
                     {
                       /// <summary>
                       /// Base Class for Razor Views which have a typed App but don't use the Model or use the typed MyModel.
                       /// </summary>
                       public abstract partial class AppRazor: AppRazor<object>
                       {
                       }
                     
                       /// <summary>
                       /// Base Class for Razor Views which have a typed App and a typed Model
                       /// </summary>
                       public abstract partial class AppRazor<TModel>: Custom.Hybrid.RazorTyped<TModel>
                       {
                     {{appSnip}}
                       }
                     }

                     """,
            Dependencies = [],
        };
        return file;
    }
}