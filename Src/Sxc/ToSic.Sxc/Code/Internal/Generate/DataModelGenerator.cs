using System.IO;
using System.Text;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Services;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Code.Internal.Generate;

/// <summary>
/// Experimental
/// </summary>
public class DataModelGenerator(ISite site, IUser user, IAppStates appStates, IAppPathsMicroSvc appPaths) : ServiceBase(SxcLogging.SxcLogName + ".DMoGen")
{
    internal CodeGenSpecs Specs { get; } = new();

    internal IUser User = user;
    internal CodeGenHelper CodeGenHelper = new(new());

    public DataModelGenerator Setup(int appId, string edition = default)
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
        var appResources = AppState.GetContentType(AppLoadConstants.TypeAppResources);
        if (appResources != null) types.Add(appResources);
        var appSettings = AppState.GetContentType(AppLoadConstants.TypeAppSettings);
        if (appSettings != null) types.Add(appSettings);

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
            sb.AppendLine(classSb.FileContents);
            sb.AppendLine();
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public void GenerateAndSaveFiles()
    {
        var logCall = Log.Fn();

        var physicalPath = GetAppCodeDataPhysicalPath();
        logCall.A($"{nameof(physicalPath)}: '{physicalPath}'");

        var classFiles = DataFiles();
        foreach (var classSb in classFiles)
        {
            logCall.A($"Writing {classSb.FileName}; Content: {classSb.FileContents.Length}");
            File.WriteAllText(Path.Combine(physicalPath, classSb.FileName), classSb.FileContents);
        }

        logCall.Done();
    }

    private string GetAppFullPath() => appPaths.Init(site, AppState).PhysicalPath;

    private string GetAppCodeDataPhysicalPath()
    {
        var appFullPath = GetAppFullPath();
        var appWithEdition = Specs.Edition.HasValue() ? Path.Combine(appFullPath, Specs.Edition) : appFullPath;

        // TODO: sanitize path because 'edition' is user provided
        var appWithEditionNormalized = new DirectoryInfo(appWithEdition).FullName;
       
        if (!Directory.Exists(appWithEditionNormalized)) throw new DirectoryNotFoundException(appWithEditionNormalized);

        var physicalPath = Path.Combine(appWithEditionNormalized, AppCodeLoader.AppCodeBase, "Data");

        // ensure the folder exists
        Directory.CreateDirectory(physicalPath);

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