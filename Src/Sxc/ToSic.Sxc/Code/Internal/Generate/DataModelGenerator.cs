using System.Text;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Services;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Code.Internal.Generate;

/// <summary>
/// Experimental
/// </summary>
public class DataModelGenerator(IUser user, IAppStates appStates) : ServiceBase(SxcLogging.SxcLogName + ".DMoGen")
{
    internal CodeGenSpecs Specs { get; } = new();
    
    internal IUser User = user;
    internal CodeGenHelper CodeGenHelper = new(new());

    public DataModelGenerator Setup(int appId, string edition)
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
        /* TODO: @STV */
    }

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

    internal CodeFragment ClassWrapper(string className, bool isAbstract, bool partial, string inherits)
    {
        var indent = CodeGenHelper.Indent(Specs.TabsClass);
        var specifiers = (isAbstract ? "abstract " : "") + (partial ? "partial " : "");
        inherits = inherits.NullOrUse(i => $": {i}");

        var start = $$"""
                      {{indent}}public {{specifiers}}class {{className}}{{inherits}}
                      {{indent}}{
                      """;
        return new("class", start, closing: $"{indent}}}\n");
    }

}