using System.IO;
using System.Reflection;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Caching.CachingMonitors;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Code.Internal.HotBuild;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class DependenciesLoader : ServiceBase
{

    public const string DependenciesFolder = "Dependencies";

    public DependenciesLoader(ILogStore logStore, ISite site, IAppStates appStates, LazySvc<IAppPathsMicroSvc> appPathsLazy, AssemblyCacheManager assemblyCacheManager, LazySvc<ThisAppCompiler> thisAppCompilerLazy) : base("Sys.AppCodeLoad")
    {
        ConnectServices(
            _logStore = logStore,
            _site = site,
            _appStates = appStates,
            _appPathsLazy = appPathsLazy,
            _assemblyCacheManager = assemblyCacheManager,
            _thisAppCompilerLazy = thisAppCompilerLazy
        );
    }
    private readonly ILogStore _logStore;
    private readonly ISite _site;
    private readonly IAppStates _appStates;
    private readonly LazySvc<IAppPathsMicroSvc> _appPathsLazy;
    private readonly AssemblyCacheManager _assemblyCacheManager;
    private readonly LazySvc<ThisAppCompiler> _thisAppCompilerLazy;


    public (List<Assembly> Assemblies, HotBuildSpec Specs) TryGetOrFallback(HotBuildSpec spec)
    {
        var l = Log.Fn<(List<Assembly>, HotBuildSpec)>(spec.ToString());
        var assemblies = TryGetAssemblyOfDependenciesFromCache(spec, Log)?.Select(r => r.Assembly).ToList();
        if (assemblies != null) return l.Return((assemblies, spec), "cached");

        assemblies = GetDependencyAssembliesOrNull(spec);
        if (assemblies != null) return l.Return((assemblies, spec), "loaded");

        if (spec.Edition.IsEmpty()) return l.Return((null, spec), "assembly empty, no edition, done");

        // try get root edition
        var rootSpec = spec.CloneWithoutEdition();
        var pairFromRoot = TryGetOrFallback(rootSpec);
        return l.Return(pairFromRoot, pairFromRoot.Assemblies == null ? "assembly without edition null" : "assembly without edition found");
    }

    private static List<AssemblyResult> TryGetAssemblyOfDependenciesFromCache(HotBuildSpec spec, ILog callerLog)
    {
        var l = callerLog.Fn<List<AssemblyResult>>($"{spec}");

        var (result, _) = AssemblyCacheManager.TryGetDependencies(spec);
        return result != null
            ? l.ReturnAsOk(result)
            : l.ReturnNull();
    }

    private List<Assembly> GetDependencyAssembliesOrNull(HotBuildSpec spec)
    {
        // Add to global history and add specs
        var logSummary = _logStore.Add(SxcLogging.SxcLogAppCodeLoader, Log);
        logSummary.UpdateSpecs(spec.ToDictionary());

        // Initial message for insights-overview
        var l = Log.Fn<List<Assembly>>($"{spec}", timer: true);

        var assemblyResults = TryLoadDependencyAssemblies(spec, logSummary);

        return assemblyResults != null 
            ? l.ReturnAsOk(assemblyResults.Select(r => r.Assembly).ToList()) 
            : l.ReturnNull("no dependencies");
    }

    private List<AssemblyResult> TryLoadDependencyAssemblies(HotBuildSpec spec, LogStoreEntry logSummary)
    {
        var l = Log.Fn<List<AssemblyResult>>($"{spec}");

        var (assemblyResults, cacheKey) = AssemblyCacheManager.TryGetDependencies(spec);
        logSummary.AddSpec("Cached", $"{assemblyResults != null} on {cacheKey}");

        if (assemblyResults != null)
            return l.ReturnAsOk(assemblyResults);

        // Get paths
        var (physicalPath, relativePath) = GetAppPaths(DependenciesFolder, spec);
        l.A($"{nameof(physicalPath)}: '{physicalPath}'; {nameof(relativePath)}: '{relativePath}'");
        logSummary.AddSpec("PhysicalPath", physicalPath);
        logSummary.AddSpec("RelativePath", relativePath);

        // missing dependencies folder
        if (!Directory.Exists(physicalPath)) 
            return l.ReturnNull($"{DependenciesFolder} folder do not exists: {physicalPath}");

        var results = new List<AssemblyResult>();
        foreach (var dependency in Directory.GetFiles(physicalPath, "*.dll"))
        {
            try
            {
                //var assembly = Assembly.Load(File.ReadAllBytes(dependency));

                var location = _thisAppCompilerLazy.Value.GetDependencyAssemblyLocations(dependency, spec);
                File.Copy(dependency, location, true);
                var assembly = Assembly.LoadFrom(location);
                results.Add(new AssemblyResult(assembly, assemblyLocations: [dependency]));
            }
            catch
            {
                // sink
            }
        }

        //logSummary.UpdateSpecs(assemblyResult.Infos);

        //if (assemblyResult.ErrorMessages.HasValue())
        //    return l.ReturnAsError(assemblyResult, assemblyResult.ErrorMessages);

        // Add dependency assemblies to cache
        _assemblyCacheManager.Add(
            cacheKey,
            results,
            changeMonitor: [new FolderChangeMonitor([physicalPath])]
        );

        return l.ReturnAsOk(results);
    }



    //private static IDictionary<string, bool> GetWatcherFolders(AssemblyResult assemblyResult, HotBuildSpec spec, string physicalPath)
    //{
    //    var watcherFolders = new Dictionary<string, bool>();

    //    // take ThisApp folder (eg. ...\edition\ThisApp)
    //    var thisAppFolder = physicalPath;
    //    IfExistsThenAdd(thisAppFolder, true);

    //    // take parent folder (eg. ...\edition)
    //    var thisAppParentFolder = Path.GetDirectoryName(thisAppFolder);
    //    if (thisAppParentFolder.IsEmpty()) return new Dictionary<string, bool>(watcherFolders);
    //    IfExistsThenAdd(thisAppParentFolder, false);

    //    // if no edition was used, then we were already in the root, and should stop now.
    //    if (spec.Edition.IsEmpty()) return new Dictionary<string, bool>(watcherFolders);

    //    // If we have an edition, and it has an assembly, we don't need to watch the root folder
    //    if (assemblyResult.HasAssembly) return new Dictionary<string, bool>(watcherFolders);

    //    // If we had an edition and no assembly, then we need to watch the root folder
    //    // we need to add more folders to watch for cache invalidation

    //    // App Root folder (eg. ...\)
    //    var appRootFolder = Path.GetDirectoryName(thisAppParentFolder);
    //    if (appRootFolder.IsEmpty()) return new Dictionary<string, bool>(watcherFolders);
    //    // Add to watcher list if it exists, otherwise exit, since we can't have subfolders
    //    if (!IfExistsThenAdd(appRootFolder, false)) return new Dictionary<string, bool>(watcherFolders);

    //    // 
    //    var appRootThisApp = Path.Combine(appRootFolder, DependenciesFolder);
    //    // Add to watcher list if it exists, otherwise exit, since we can't have subfolders
    //    if (!IfExistsThenAdd(appRootThisApp, true)) return new Dictionary<string, bool>(watcherFolders);

    //    // all done
    //    return new Dictionary<string, bool>(watcherFolders);

    //    // Helper to add and return info if it exists
    //    bool IfExistsThenAdd(string folder, bool watchSubfolders)
    //    {
    //        if (!Directory.Exists(folder)) return false;
    //        watcherFolders.Add(folder, watchSubfolders);
    //        return true;
    //    }   
    //}

    private (string physicalPath, string relativePath) GetAppPaths(string folder, HotBuildSpec spec)
    {
        var l = Log.Fn<(string physicalPath, string relativePath)>($"{spec}");
        var appPaths = _appPathsLazy.Value.Init(_site, _appStates.GetReader(spec.AppId));
        var folderWithEdition = folder.HasValue() 
            ? (spec.Edition.HasValue() ? Path.Combine(spec.Edition, folder) : folder)
            : spec.Edition;
        var physicalPath = Path.Combine(appPaths.PhysicalPath, folderWithEdition);
        l.A($"{nameof(physicalPath)}: '{physicalPath}'");
        var relativePath = Path.Combine(appPaths.RelativePath, folderWithEdition);
        l.A($"{nameof(relativePath)}: '{relativePath}'");
        return l.ReturnAsOk((physicalPath, relativePath));
    }

    // Idea: put dll in the App/bin folder, for VS Intellisense - ATM not relevant
    //private static void AssembliesDelete(IEnumerable<string> list)
    //{
    //    if (list == null) return;
    //    foreach (var assembly in list)
    //    {
    //        try
    //        {
    //            File.Delete(assembly);
    //        }
    //        catch
    //        {
    //            // ignore
    //        }
    //    }
    //}

    // Idea: put dll in the App/bin folder, for VS Intellisense - ATM not relevant
    //private static void CopyAssemblyForRefs(string source, string destination)
    //{
    //    if (!File.Exists(source)) return;
    //    var destinationFolder = Path.GetDirectoryName(destination);
    //    if (destinationFolder != null) Directory.CreateDirectory(destinationFolder);
    //    File.Copy(source, destination, true);
    //}
}