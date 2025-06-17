using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Data.Ancestors.Sys;
using ToSic.Eav.Data.ContentTypes.Sys;
using ToSic.Eav.Data.Sys;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Code.Generate.Internal;

/// <summary>
/// Base class for C# models (data/custom) generators.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal abstract class CSharpModelsGeneratorBase(IUser user, IAppReaderFactory appReadFac, string logName)
    : CSharpGeneratorBase(user, appReadFac, logName), IFileGenerator
{
    internal protected CSharpCodeSpecs Specs { get; protected set; } = new();
    internal protected CSharpGeneratorHelper CodeGenHelper { get; protected set; }

    // Abstract properties to be implemented by derived classes
    public new abstract string Description { get; }
    public new abstract string DescriptionHtml { get; }
    protected abstract string GeneratedSetName { get; }

    // OutputType is common
    public new string OutputType => "DataModel";

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

        Specs.ExportedContentContentTypes = types;

        CodeGenHelper = new(Specs, Log);
    }

    protected abstract IGeneratedFile CreateFileGenerator(IContentType type, string className);

    public new IGeneratedFileSet[] Generate(IFileGeneratorSpecs specs)
    {
        Setup(specs);

        var classFiles = Specs.ExportedContentContentTypes
            .Select(t => CreateFileGenerator(t, t.Name?.Replace("-", "")))
            .Where(f => f != null) // Ensure no null files are added
            .ToList();

        var result = new GeneratedFileSet
        {
            Name = GeneratedSetName,
            Description = this.Description, // This will call the derived implementation
            Generator = $"{this.Name} v{this.Version}", // Name and Version from CSharpGeneratorBase
            Path = GenerateConstants.PathToAppCode,
            Files = classFiles.Cast<IGeneratedFile>().ToList()
        };
        return [result];
    }
}
