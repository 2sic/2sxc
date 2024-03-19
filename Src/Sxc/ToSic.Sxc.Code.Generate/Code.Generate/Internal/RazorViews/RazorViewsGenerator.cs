using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using static ToSic.Sxc.Internal.SxcLogging;

namespace ToSic.Sxc.Code.Generate.Internal.RazorViews;

/// <summary>
/// Experimental
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class RazorViewsGenerator(IUser user, IAppStates appStates)
    : CSharpGeneratorBase(user, appStates, SxcLogName + ".RzrVGn"), IFileGenerator
{
    internal CSharpCodeSpecs Specs { get; } = new();

    #region Information for the interface

    public string Description => "Generates Razor Views in the AppCode/Razor folder (dummy, not working yet!)";

    public string DescriptionHtml => $"EXPERIMENTAL The {Name} will generate <code>AppRazor.Generated.cs</code> files in the <code>AppCode/Razor</code> folder.";

    public string OutputType => "RazorView";

    #endregion


    public IGeneratedFileSet[] Generate(IFileGeneratorSpecs specs)
    {
        var cSharpSpecs = BuildSpecs(specs);

        var file = new GeneratedFile
        {
            FileName = "AppRazor.Generated.cs",
            Path = "Razor",
            Body = $$"""
                   // DO NOT MODIFY THIS FILE - IT IS AUTO-GENERATED
                   // All the classes are partial, so you can extend them in a separate file.
                   
                   {{CSharpGeneratorHelper.GeneratorHeader(this, cSharpSpecs, User.Name)}}

                   using AppCode.Data;
                   using ToSic.Sxc.Apps;

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
                       /// <summary>
                       /// Typed App with typed Settings & Resources
                       /// </summary>
                       public new IAppTyped<AppSettings, AppResources> App => _app ??= Customize.App<AppSettings, AppResources>();
                       private IAppTyped<AppSettings, AppResources> _app;
                       
                     }
                   }

                   """,
            Dependencies = [],
        };

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

}