using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.Data.Sys.Ancestors;
using ToSic.Eav.Data.Sys.ContentTypes;
using ToSic.Sys.Performance;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Code.Generate.Sys;

/// <summary>
/// Base class for C# models (data/custom) generators.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal abstract class CSharpModelsGeneratorBase(IUser user, IAppReaderFactory appReadFac, string logName)
    : CSharpGeneratorBase(user, appReadFac, logName), IFileGenerator
{
    protected internal CSharpCodeSpecs Specs { get; protected set; } = new();
    protected internal CSharpGeneratorHelper CodeGenHelper { get; protected set; } = null!;

    // Abstract properties to be implemented by derived classes
    public abstract string Description { get; }
    public abstract string DescriptionHtml { get; }
    protected abstract string GeneratedSetName { get; }

    // OutputType is common
    public string OutputType => "DataModel";

    protected internal abstract CSharpCodeSpecs BuildDerivedSpecs(IFileGeneratorSpecs parameters);

    protected virtual void Setup(IFileGeneratorSpecs parameters)
    {
        Specs = BuildDerivedSpecs(parameters);
        var appContentTypes = Specs.AppContentTypes;

        // Prepare Content Types and add to Specs, so the generators know what is available
        // Generate classes for all types in scope Default
        var types = appContentTypes.ContentTypes
            .OfScope(ScopeConstants.Default)
            .ToList();
        appContentTypes.TryGetContentType(AppLoadConstants.TypeAppResources).DoIfNotNull(types.Add);
        appContentTypes.TryGetContentType(AppLoadConstants.TypeAppSettings).DoIfNotNull(types.Add);

        var appConfigTypes = appContentTypes.ContentTypes
            .OfScope(ScopeConstants.SystemConfiguration)
            .Where(ct => !ct.HasAncestor())
            .ToList();

        // Fix Bug introduced in v19.03.01 or so
        // In that UI it accidentally created new content-types called AppResources or AppSettings
        // In the SystemConfiguration scope. These must really be ignored.
        // In addition, to avoid any other naming conflicts, we'll just skip all the names that were already used.
        var typeNames = types
            .Select(t => t.Name)
            .ToList();
        appConfigTypes = appConfigTypes
            .Where(ct => !typeNames.Contains(ct.Name))
            .ToList();

        types.AddRange(appConfigTypes);

        Specs = Specs with { ExportedContentContentTypes = types };

        CodeGenHelper = new(Specs, Log);
    }

    protected abstract IGeneratedFile? CreateFileGenerator(IContentType type, string className);

    public IGeneratedFileSet[] Generate(IFileGeneratorSpecs specs)
    {
        Setup(specs);

        var classFiles = Specs.ExportedContentContentTypes
            .Select(t => CreateFileGenerator(t, (t.Name ?? "").Replace("-", "")))
            .Where(f => f != null) // Ensure no null files are added
            .ToListOpt();

        var result = new GeneratedFileSet
        {
            Name = GeneratedSetName,
            Description = Description, // This will call the derived implementation
            Generator = $"{Name} v{Version}", // Name and Version from CSharpGeneratorBase
            Path = GenerateConstants.PathToAppCode,
            Files = classFiles.Cast<IGeneratedFile>().ToList()
        };
        return [result];
    }
}
