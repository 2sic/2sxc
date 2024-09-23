using ToSic.Eav.Apps;
using ToSic.Eav.Context;

namespace ToSic.Sxc.Code.Generate.Internal.CSharpBaseClasses;

/// <summary>
/// Experimental
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CSharpServicesGenerator(IUser user, IAppReaderFactory appReadFac)
    : CSharpGeneratorBase(user, appReadFac, SxcLogName + ".SvcGen"), IFileGenerator
{
    #region Information for the interface

    public string Description => "Generates CSharp Service Base Classes in the AppCode/Services folder";

    public string DescriptionHtml => $"The {Name} will generate <code>ServiceBase.Generated.cs</code> files in the <code>AppCode/Services</code> folder. <br> IMPORTANT: Requires that Content-Types were generated.";

    public string OutputType => "RazorView";// "CSharpService";

    #endregion


    public IGeneratedFileSet[] Generate(IFileGeneratorSpecs specs)
    {
        var cSharpSpecs = BuildSpecs(specs);

        var file = CSharpServiceBase(cSharpSpecs, "ServiceBase", "Custom.Hybrid.CodeTyped", "Services");

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

    internal GeneratedFile CSharpServiceBase(CSharpCodeSpecs cSharpSpecs, string className, string baseClass, string nsAndPath)
    {
        var (_, appSnip, allUsings) = BaseClassHelper.BaseClassTools(cSharpSpecs);

        var file = new GeneratedFile
        {
            FileName = $"{className}.Generated.cs",
            Path = nsAndPath,
            Body = $$"""
                     {{CSharpGeneratorHelper.DoNotModifyMessage}}
                     
                     {{CSharpGeneratorHelper.GeneratorHeader(this, cSharpSpecs, User.Name)}}
                     
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