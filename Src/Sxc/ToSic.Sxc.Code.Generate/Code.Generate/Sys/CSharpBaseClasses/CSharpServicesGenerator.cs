using ToSic.Eav.Apps;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Code.Generate.Sys.CSharpBaseClasses;

/// <summary>
/// Experimental
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class CSharpServicesGenerator(IUser user, IAppReaderFactory appReadFac)
    : ServiceBase($"{SxcLogName}.SvcGen"), IFileGenerator
{
    #region Information for the interface

    public FileGeneratorSpecs GeneratorSpecs => field ??= new(this)
    {
        OutputLanguage = "CSharp",
        OutputType = "CSharpService",
        Description = "Generates CSharp Service Base Classes in the AppCode/Services folder",
        DescriptionHtml = $"The {GetType().Name} will generate <code>[Prefix]ServiceBase[Suffix].Generated.cs</code> files in the <code>AppCode/Services</code> folder. <br> IMPORTANT: Requires that Content-Types were generated."
    };

    #endregion


    public IGeneratedFileSet[] Generate(GenerateFileSetSpecs specs)
    {
        var appReader = appReadFac.Get(specs.AppId);
        var cSharpSpecs = new CSharpCodeSpecs
        {
            AppName = appReader.Specs.Name,
            SetSpecs = specs,
        };

        var file = GenerateCSharpService(GeneratorSpecs, cSharpSpecs, user, "ServiceBase", "Custom.Hybrid.CodeTyped", "Services", Log);

        var result = GeneratedFileSet.FromSpecs(GeneratorSpecs, "C# Services", [file]);
        return [result];
    }

    internal static GeneratedFile GenerateCSharpService(FileGeneratorSpecs genSpecs, CSharpCodeSpecs cSharpSpecs, IUser user, string baseName, string baseClass, string nsAndPath, ILog log)
    {
        var className = $"{cSharpSpecs.SetSpecs.Prefix}{baseName}{cSharpSpecs.SetSpecs.Suffix}";
        var (_, appSnip, allUsings) = BaseClassHelper.BaseClassTools(cSharpSpecs, log);

        var file = new GeneratedFile
        {
            FileName = $"{className}.Generated.cs",
            Path = nsAndPath,
            Body = $$"""
                     {{CSharpGeneratorHelper.DoNotModifyMessage}}
                     
                     {{CSharpGeneratorHelper.GeneratorHeader(genSpecs, cSharpSpecs, user.Name)}}
                     
                     {{string.Join("\n", allUsings)}}
                     
                     namespace AppCode.{{nsAndPath}}
                     {
                       /// <summary>
                       /// Base Class for Services which have a typed App.
                       /// </summary>
                       public abstract partial class {{className}}: {{baseClass}}
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
