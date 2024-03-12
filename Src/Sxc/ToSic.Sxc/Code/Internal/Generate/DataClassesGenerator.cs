using System.IO;
using System.Text;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
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
public class DataClassesGenerator(ISite site, IUser user, IAppStates appStates, IAppPathsMicroSvc appPaths) : ServiceBase(SxcLogName + ".DMoGen")
{
    internal CodeGenSpecs Specs { get; } = new();

    internal IUser User = user;
    internal CodeGenHelper CodeGenHelper = new(new());

    public DataClassesGenerator Setup(int appId, string edition = default)
    {
        if (edition.HasValue())
            Specs.Edition = edition;

        // Prepare App State and add to Specs
        var appCache = appStates.GetCacheState(appId);
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
        return this;
    }

    public IAppState AppState { get; private set; }

    public string Dump()
    {
        // TODO: @2dm
        // - check Equals of the new objects
        // - check serialization
        var sb = new StringBuilder();

        var classFiles = DataFiles();
        foreach (var classSb in classFiles)
        {
            sb.AppendLine($"// ----------------------- file: {classSb.FileName} ----------------------- ");
            sb.AppendLine(classSb.Body);
            sb.AppendLine();
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public void GenerateAndSaveFiles()
    {
        var l = Log.Fn();

        var physicalPath = GetAppCodeDataPhysicalPath();
        l.A($"{nameof(physicalPath)}: '{physicalPath}'");

        var classFiles = DataFiles();
        foreach (var classSb in classFiles)
        {
            l.A($"Writing {classSb.FileName}; Path: {classSb.Path}; Content: {classSb.Body.Length}");

            var addPath = classSb.Path ?? "";
            if (addPath.StartsWith("/") || addPath.StartsWith("\\") || addPath.EndsWith("/") || addPath.EndsWith("\\") || addPath.Contains(".."))
                throw new($"Invalid path '{addPath}' in class '{classSb.FileName}' - contains invalid path like '..' or starts/ends with a slash.");

            var basePath = Path.Combine(physicalPath, classSb.Path);
            
            // ensure the folder for the file exists - it could be different for each file
            Directory.CreateDirectory(basePath);

            var fullPath = Path.Combine(basePath, classSb.FileName);
            File.WriteAllText(fullPath, classSb.Body);
        }

        l.Done();
    }

    private string GetAppFullPath() => appPaths.Init(site, AppState).PhysicalPath;

    private string GetAppCodeDataPhysicalPath()
    {
        var appFullPath = GetAppFullPath();
        var appWithEdition = Specs.Edition.HasValue() ? Path.Combine(appFullPath, Specs.Edition) : appFullPath;

        // TODO: sanitize path because 'edition' is user provided
        var appWithEditionNormalized = new DirectoryInfo(appWithEdition).FullName;
       
        if (!Directory.Exists(appWithEditionNormalized))
            throw new DirectoryNotFoundException(appWithEditionNormalized);

        var physicalPath = Path.Combine(appWithEditionNormalized, AppCodeLoader.AppCodeBase);
        return physicalPath;
    }

    internal string GetPathToDotAppJson() => Path.Combine(GetAppFullPath(), Constants.AppDataProtectedFolder, Constants.AppJson);


    internal List<CodeFileRaw> DataFiles()
    {
        var classFiles = Specs.ExportedContentContentTypes
            .Select(t => new DataClassGenerator(this, t, t.Name?.Replace("-", "")).PrepareFile())
            .ToList();
        return classFiles;
    }


    internal CodeFragment NamespaceWrapper(string @namespace)
    {
        return new("namespace", $"{CodeGenHelper.Indent(Specs.TabsNamespace)}namespace {@namespace}" + "\n{", closing: "}");
    }

    internal CodeFragment ClassWrapper(string className, bool isAbstract, bool isPartial, string inherits)
    {
        var indent = CodeGenHelper.Indent(Specs.TabsClass);
        var specifiers = (isAbstract ? "abstract " : "") + (isPartial ? "partial " : "");
        inherits = inherits.NullOrGetWith(i => $": {i}");

        var start = $$"""
                      {{indent}}public {{specifiers}}class {{className}}{{inherits}}
                      {{indent}}{
                      """;
        return new("class", start, closing: $"{indent}}}\n");
    }

}