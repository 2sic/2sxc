using ToSic.Eav.Apps;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Code.Generate.Sys.CSharpBaseClasses;

/// <summary>
/// Experimental
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class RazorViewsGenerator(IUser user, IAppReaderFactory appReadFac)
    : ServiceBase($"{SxcLogName}.RzrVGn"), IFileGenerator
{
    #region Information for the interface

    public FileGeneratorSpecs GeneratorSpecs => field ??= new(this)
    {
        Description = "Generates Razor Views in the AppCode/Razor folder",
        DescriptionHtml = $"The {GetType().Name} will generate <code>[Prefix]AppRazor[Suffix].Generated.cs</code> files in the <code>AppCode/Razor</code> folder. <br> IMPORTANT: Requires that Content-Types were generated.",
        OutputLanguage = "CSharp",
        OutputType = "RazorView"
    };

    #endregion


    public IGeneratedFileSet[] Generate(IGenerateFileSetSpecs specs)
    {
        var cSharpSpecs = CSharpGeneratorShared.BuildSpecs(appReadFac.Get(specs.AppId).Specs.Name, specs);
        var file = AppRazors(cSharpSpecs, "AppRazor");

        var result = GeneratedFileSet.FromSpecs(GeneratorSpecs, "C# Razor Views", [file]);
        return [result];
    }

    private GeneratedFile AppRazors(CSharpCodeSpecs cSharpSpecs, string baseName)
    {
        var className = $"{cSharpSpecs.Prefix}{baseName}{cSharpSpecs.Suffix}";
        var (_, appSnip, allUsings) = BaseClassHelper.BaseClassTools(cSharpSpecs, Log);

        var file = new GeneratedFile
        {
            FileName = $"{className}.Generated.cs",
            Path = "Razor",
            Body = $$"""
                     {{CSharpGeneratorHelper.DoNotModifyMessage}}

                     {{CSharpGeneratorHelper.GeneratorHeader(GeneratorSpecs, cSharpSpecs, user.Name)}}

                     {{string.Join("\n", allUsings)}}

                     namespace AppCode.Razor
                     {
                       /// <summary>
                       /// Base Class for Razor Views which have a typed App but don't use the Model or use the typed MyModel.
                       /// </summary>
                       public abstract partial class {{className}}: {{className}}<object>
                       {
                       }
                     
                       /// <summary>
                       /// Base Class for Razor Views which have a typed App and a typed Model
                       /// </summary>
                       public abstract partial class {{className}}<TModel>: Custom.Hybrid.RazorTyped<TModel>
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
