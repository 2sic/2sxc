using System.IO;
using System.Reflection;
using System.Runtime.Caching;
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
public class ThisAppLoader : ServiceBase
{
    public const string ThisAppBase = "ThisApp";
    //public const string ThisAppBinFolder = "bin";

    public ThisAppLoader(ILogStore logStore, ISite site, IAppStates appStates, LazySvc<IAppPathsMicroSvc> appPathsLazy, LazySvc<ThisAppCompiler> thisAppCompilerLazy, AssemblyCacheManager assemblyCacheManager) : base("Sys.AppCodeLoad")
    {
        ConnectServices(
            _logStore = logStore,
            _site = site,
            _appStates = appStates,
            _appPathsLazy = appPathsLazy,
            _thisAppCompilerLazy = thisAppCompilerLazy,
            _assemblyCacheManager = assemblyCacheManager
        );
    }
    private readonly ILogStore _logStore;
    private readonly ISite _site;
    private readonly IAppStates _appStates;
    private readonly LazySvc<IAppPathsMicroSvc> _appPathsLazy;
    private readonly LazySvc<ThisAppCompiler> _thisAppCompilerLazy;
    private readonly AssemblyCacheManager _assemblyCacheManager;


    public (Assembly Assembly, HotBuildSpec Specs) TryGetOrFallback(HotBuildSpec spec)
    {
        var l = Log.Fn<(Assembly, HotBuildSpec)>(spec.ToString());
        var assembly = TryGetAssemblyOfThisAppFromCache(spec, Log)?.Assembly;
        if (assembly != null) return l.Return((assembly, spec), "cached");

        assembly = GetThisAppAssemblyOrThrow(spec);
        if (assembly != null) return l.Return((assembly, spec), "compiled");

        if (spec.Edition.IsEmpty()) return l.Return((null, spec), "assembly empty, no edition, done");

        // try get root edition
        var rootSpec = spec.CloneWithoutEdition();
        var pairFromRoot = TryGetOrFallback(rootSpec);
        return l.Return(pairFromRoot, pairFromRoot.Assembly == null ? "assembly without edition null" : "assembly without edition found");
    }

    public static AssemblyResult TryGetAssemblyOfThisAppFromCache(HotBuildSpec spec, ILog callerLog)
    {
        var l = callerLog.Fn<AssemblyResult>($"{spec}");

        var (result, _) = AssemblyCacheManager.TryGetThisApp(spec);
        return result != null
            ? l.ReturnAsOk(result)
            : l.ReturnNull();
    }

    public Assembly GetThisAppAssemblyOrThrow(HotBuildSpec spec)
    {
        // Add to global history and add specs
        var logSummary = _logStore.Add(SxcLogging.SxcLogAppCodeLoader, Log);
        logSummary.UpdateSpecs(spec.ToDictionary());

        // Initial message for insights-overview
        var l = Log.Fn<Assembly>($"{spec}", timer: true);

        var assemblyResults = TryLoadAppAssembly(spec, logSummary);

        // All OK (no errors) - return
        if (string.IsNullOrEmpty(assemblyResults?.ErrorMessages))
            return l.ReturnAsOk(assemblyResults?.Assembly);
        
        // Problems - log and throw
        l.ReturnAsError(null, assemblyResults.ErrorMessages);
        throw new(assemblyResults.ErrorMessages);
    }

    private AssemblyResult TryLoadAppAssembly(HotBuildSpec spec, LogStoreEntry logSummary)
    {
        var l = Log.Fn<AssemblyResult>($"{spec}");

        var (result, cacheKey) = AssemblyCacheManager.TryGetThisApp(spec);
        logSummary.AddSpec("Cached", $"{result != null} on {cacheKey}");

        if (result != null)
            return l.ReturnAsOk(result);

        // Get paths
        var (physicalPath, relativePath) = GetAppPaths(ThisAppBase, spec);
        l.A($"{nameof(physicalPath)}: '{physicalPath}'; {nameof(relativePath)}: '{relativePath}'");
        logSummary.AddSpec("PhysicalPath", physicalPath);
        logSummary.AddSpec("RelativePath", relativePath);
 
        var assemblyResult = _thisAppCompilerLazy.Value.GetThisApp(relativePath, spec);

        logSummary.UpdateSpecs(assemblyResult.Infos);

        if (assemblyResult.ErrorMessages.HasValue())
            return l.ReturnAsError(assemblyResult, assemblyResult.ErrorMessages);

        assemblyResult.WatcherFolders = GetWatcherFolders(assemblyResult, spec, physicalPath);
        l.A("Folders to watch:");
        foreach (var watcherFolder in assemblyResult.WatcherFolders)
            l.A($"- '{watcherFolder}'");

        // Idea: put dll in the App/bin folder, for VS Intellisense - ATM not relevant
        //var (refsAssemblyPath, _) = GetAppPaths(ThisAppBinFolder, spec);
        //if (assemblyResult.HasAssembly)
        //    CopyAssemblyForRefs(assemblyResult.AssemblyLocations[1], Path.Combine(refsAssemblyPath, ThisAppCompiler.ThisAppDll));

        // Add compiled assembly to cache
        _assemblyCacheManager.Add(
            cacheKey,
            assemblyResult,
            changeMonitor: new ChangeMonitor[] { new FolderChangeMonitor(assemblyResult.WatcherFolders) }
            // Idea: put dll in the App/bin folder, for VS Intellisense - ATM not relevant
            // updateCallback: _ => AssembliesDelete(assemblyResult.AssemblyLocations.Append(refsAssemblyPath))
        );

        return l.ReturnAsOk(assemblyResult);
    }

    private static IDictionary<string, bool> GetWatcherFolders(AssemblyResult assemblyResult, HotBuildSpec spec, string physicalPath)
    {
        var watcherFolders = new Dictionary<string, bool>();

        // take ThisApp folder (eg. ...\edition\ThisApp)
        var thisAppFolder = physicalPath;
        IfExistsThenAdd(thisAppFolder, true);

        // take parent folder (eg. ...\edition)
        var thisAppParentFolder = Path.GetDirectoryName(thisAppFolder);
        if (thisAppParentFolder.IsEmpty()) return new Dictionary<string, bool>(watcherFolders);
        IfExistsThenAdd(thisAppParentFolder, false);

        // if no edition was used, then we were already in the root, and should stop now.
        if (spec.Edition.IsEmpty()) return new Dictionary<string, bool>(watcherFolders);

        // If we have an edition, and it has an assembly, we don't need to watch the root folder
        if (assemblyResult.HasAssembly) return new Dictionary<string, bool>(watcherFolders);

        // If we had an edition and no assembly, then we need to watch the root folder
        // we need to add more folders to watch for cache invalidation

        // App Root folder (eg. ...\)
        var appRootFolder = Path.GetDirectoryName(thisAppParentFolder);
        if (appRootFolder.IsEmpty()) return new Dictionary<string, bool>(watcherFolders);
        // Add to watcher list if it exists, otherwise exit, since we can't have subfolders
        if (!IfExistsThenAdd(appRootFolder, false)) return new Dictionary<string, bool>(watcherFolders);

        // 
        var appRootThisApp = Path.Combine(appRootFolder, ThisAppBase);
        // Add to watcher list if it exists, otherwise exit, since we can't have subfolders
        if (!IfExistsThenAdd(appRootThisApp, true)) return new Dictionary<string, bool>(watcherFolders);

        // all done
        return new Dictionary<string, bool>(watcherFolders);

        // Helper to add and return info if it exists
        bool IfExistsThenAdd(string folder, bool watchSubfolders)
        {
            if (!Directory.Exists(folder)) return false;
            watcherFolders.Add(folder, watchSubfolders);
            return true;
        }   
    }

    public (string physicalPath, string relativePath) GetAppPaths(string folder, HotBuildSpec spec)
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