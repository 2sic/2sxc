using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Data.Shared;
using ToSic.Eav.Plumbing;
using static ToSic.Sxc.Internal.SxcLogging;

namespace ToSic.Sxc.Code.Generate.Internal;

/// <summary>
/// Experimental
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CSharpDataModelsGenerator(IUser user, IAppStates appStates)
    : CSharpGeneratorBase(user, appStates, SxcLogName + ".DMoGen"), IFileGenerator
{
    internal CSharpCodeSpecs Specs { get; private set; } = new();

    internal CSharpGeneratorHelper CodeGenHelper { get; private set; }

    #region Information for the interface


    public string Description => "Generates C# Data Classes for the AppCode/Data folder";

    public string DescriptionHtml => $"The {Name} will generate <code>[TypeName].Generated.cs</code> files in the <code>AppCode/Data</code> folder.";

    public string OutputType => "DataModel";

    #endregion

    private void Setup(IFileGeneratorSpecs parameters)
    {
        Specs = BuildSpecs(parameters);
        var appState = Specs.AppState;

        // Prepare Content Types and add to Specs, so the generators know what is available
        // Generate classes for all types in scope Default
        var types = appState.ContentTypes.OfScope(Scopes.Default).ToList();
        appState.GetContentType(AppLoadConstants.TypeAppResources).DoIfNotNull(types.Add);
        appState.GetContentType(AppLoadConstants.TypeAppSettings).DoIfNotNull(types.Add);

        var appConfigTypes = appState.ContentTypes
            .OfScope(Scopes.SystemConfiguration)
            .Where(ct => !ct.HasAncestor())
            .ToList();

        types.AddRange(appConfigTypes);

        Specs.ExportedContentContentTypes = types;

        CodeGenHelper = new(Specs);
    }



    public IGeneratedFileSet[] Generate(IFileGeneratorSpecs specs)
    {
        Setup(specs);

        var classFiles = Specs.ExportedContentContentTypes
                .Select(t => new CSharpDataModelGenerator(this, t, t.Name?.Replace("-", "")).PrepareFile())
                .ToList();

        var result = new GeneratedFileSet
        {
            Name = "C# Data Classes",
            Description = Description,
            Generator = $"{Name} v{Version}",
            Path = GenerateConstants.PathToAppCode,
            Files = classFiles.Cast<IGeneratedFile>().ToList()
        };
        return [result];
    }

}