using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Lib.Services;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Internal;
using static ToSic.Sxc.Internal.SxcLogging;

namespace ToSic.Sxc.Code.Generate.Internal.RazorViews;

/// <summary>
/// Experimental
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class RazorViewsGenerator(IUser user, IAppStates appStates)
    : ServiceBase(SxcLogName + ".RzrVGn"), IFileGenerator
{
    internal CSharpCodeSpecs Specs { get; } = new();

    //internal IUser User = user;
    //internal CSharpGeneratorHelper CodeGenHelper { get; private set; }

    #region Information for the interface

    public string NameId => GetType().FullName;

    public string Name => nameof(RazorViewsGenerator);

    public string Version => SharedAssemblyInfo.AssemblyVersion;

    public string Description => "Generates Razor Views in the AppCode/Razor folder (dummy, not working yet!)";

    public string DescriptionHtml => $"EXPERIMENTAL The {Name} will generate <code>AppRazorGenerated.cs</code> files in the <code>AppCode/Razor</code> folder.";

    public string OutputLanguage => "CSharp";
    public string OutputType => "RazorView";

    #endregion

    //private void Setup(IFileGeneratorSpecs parameters)
    //{
    //    if (parameters.Edition.HasValue())
    //        Specs.Edition = parameters.Edition;

    //    // Prepare App State and add to Specs
    //    var appCache = appStates.GetCacheState(parameters.AppId);
    //    AppState = appStates.ToReader(appCache);
    //    Specs.AppState = AppState;
    //    Specs.AppName = AppState.Name;

    //    // Prepare Content Types and add to Specs, so the generators know what is available
    //    // Generate classes for all types in scope Default
    //    var types = AppState.ContentTypes.OfScope(Scopes.Default).ToList();
    //    AppState.GetContentType(AppLoadConstants.TypeAppResources).DoIfNotNull(types.Add);
    //    AppState.GetContentType(AppLoadConstants.TypeAppSettings).DoIfNotNull(types.Add);

    //    var appConfigTypes = AppState.ContentTypes
    //        .OfScope(Scopes.SystemConfiguration)
    //        .Where(ct => !ct.HasAncestor())
    //        .ToList();

    //    types.AddRange(appConfigTypes);

    //    Specs.ExportedContentContentTypes = types;
    //    CodeGenHelper = new(Specs);
    //}

    public IAppState AppState { get; private set; }


    public IGeneratedFileSet[] Generate(IFileGeneratorSpecs specs)
    {
        //Setup(specs);

        //var classFiles = Specs.ExportedContentContentTypes
        //        .Select(t => new CSharpDataModelGenerator(this, t, t.Name?.Replace("-", "")).PrepareFile())
        //        .ToList();

        var result = new GeneratedFileSet
        {
            Name = "C# Razor Views",
            Description = Description,
            Generator = $"{Name} v{Version}",
            Path = $"{GenerateConstants.PathPlaceholderAppRoot}/{GenerateConstants.PathPlaceholderEdition}/{AppCodeLoader.AppCodeBase}",
            Files = [], // classFiles.Cast<IGeneratedFile>().ToList()
        };
        return [result];
    }

}