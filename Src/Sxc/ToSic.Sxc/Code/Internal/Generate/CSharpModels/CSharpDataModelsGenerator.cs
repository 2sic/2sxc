using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Data.Shared;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Services;
using ToSic.Sxc.Code.Internal.HotBuild;

namespace ToSic.Sxc.Code.Internal.Generate;

/// <summary>
/// Experimental
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CSharpDataModelsGenerator(IUser user, IAppStates appStates)
    : ServiceBase(SxcLogName + ".DMoGen"), IFileGenerator
{
    internal CSharpCodeSpecs Specs { get; } = new();

    internal IUser User = user;
    internal CSharpGeneratorHelper CodeGenHelper { get; private set; }

    #region Information for the interface

    public string NameId => GetType().FullName;

    public string Name => nameof(CSharpDataModelGenerator);

    public string Version => SharedAssemblyInfo.AssemblyVersion;

    public string Description => "Generates C# Data Classes for the AppCode/Data folder";

    public string OutputLanguage => "CSharp";
    public string OutputType => "DataModel";

    #endregion

    public CSharpDataModelsGenerator Setup(GenerateParameters parameters)
    {
        if (parameters.Edition.HasValue())
            Specs.Edition = parameters.Edition;

        // Prepare App State and add to Specs
        var appCache = appStates.GetCacheState(parameters.AppId);
        AppState = appStates.ToReader(appCache);
        Specs.AppState = AppState;
        Specs.AppName = AppState.Name;

        // Prepare Content Types and add to Specs, so the generators know what is available
        // Generate classes for all types in scope Default
        var types = AppState.ContentTypes.OfScope(Scopes.Default).ToList();
        AppState.GetContentType(AppLoadConstants.TypeAppResources).DoIfNotNull(types.Add);
        AppState.GetContentType(AppLoadConstants.TypeAppSettings).DoIfNotNull(types.Add);

        var appConfigTypes = AppState.ContentTypes
            .OfScope(Scopes.SystemConfiguration)
            .Where(ct => !ct.HasAncestor())
            .ToList();

        types.AddRange(appConfigTypes);

        Specs.ExportedContentContentTypes = types;
        CodeGenHelper = new(Specs);
        return this;
    }

    public IAppState AppState { get; private set; }


    public ICodeFileBundle[] Generate()
    {

        var classFiles = Specs.ExportedContentContentTypes
            .Select(t => new CSharpDataModelGenerator(this, t, t.Name?.Replace("-", "")).PrepareFile())
            .ToList();

        var result = new CodeFileBundle
        {
            Name = "C# Data Classes",
            Description = Description,
            Generator = $"{Name} v{Version}",
            Path = $"{GenerateConstants.AppRootFolderPlaceholder}/{GenerateConstants.EditionPlaceholder}/{AppCodeLoader.AppCodeBase}",
            Files = classFiles.Cast<ICodeFile>().ToList()
        };
        return [result];
    }

}