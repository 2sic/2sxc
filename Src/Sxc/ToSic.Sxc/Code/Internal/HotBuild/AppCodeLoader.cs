using System.Collections.Concurrent;
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

namespace ToSic.Sxc.Code.Internal.HotBuild;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppCodeLoader(
    ILogStore logStore,
    ISite site,
    IAppStates appStates,
    LazySvc<IAppPathsMicroSvc> appPathsLazy,
    LazySvc<AppCodeCompiler> appCompilerLazy,
    AssemblyCacheManager assemblyCacheManager)
    : ServiceBase("Sys.AppCodeLoad",
        connect: [logStore, site, appStates, appPathsLazy, appCompilerLazy, assemblyCacheManager])
{
    public const string AppCodeBase = "AppCode";


    /// <summary>
    /// Try to get the app code - first of the edition, then of the root.
    /// </summary>
    /// <param name="spec"></param>
    /// <returns></returns>
    public (AssemblyResult AssemblyResult, HotBuildSpec Specs) GetAppCode(HotBuildSpec spec)
    {
        var l = Log.Fn<(AssemblyResult, HotBuildSpec)>(spec.ToString());
        var firstRound = GetOrBuildAppCode(spec);
        if (firstRound.AssemblyResult?.Assembly != null)
            return l.Return(firstRound, $"AppCode for '{spec.EditionToLog}'.");

        if (spec.Edition.IsEmpty())
            return l.Return(firstRound, $"No AppCode for '{spec.EditionToLog}', done.");

        // try get root edition
        var rootSpec = spec.CloneWithoutEdition();
        var root = GetOrBuildAppCode(rootSpec);
        return l.Return(root, $"AppCode in '{root.Specs.EditionToLog}'." + (root.AssemblyResult?.Assembly == null ? ", null." : ""));
    }

    /// <summary>
    /// AppCode - get from cache or build - if there is any code to build.
    /// Will throw exceptions if compile fails, but not if there is no code to compile.
    /// </summary>
    /// <param name="spec"></param>
    /// <returns></returns>
    public (AssemblyResult AssemblyResult, HotBuildSpec Specs) GetOrBuildAppCode(HotBuildSpec spec)
    {
        var l = Log.Fn<(AssemblyResult, HotBuildSpec)>(spec.ToString());

        // Check cache first
        var assemblyResult = assemblyCacheManager.TryGetAppCode(spec).AssemblyResult;
        if (assemblyResult?.Assembly != null)
            return l.Return((assemblyResult, spec), $"AppCode from cached for '{spec.EditionToLog}'.");

        // Try to compile
        assemblyResult = TryBuildAppCodeAndLog(spec);
        return l.Return((assembly: assemblyResult, spec), "AppCode " + (assemblyResult != null ? "compiled" : "not compiled") + $" for '{spec.EditionToLog}'.");
    }

    /// <summary>
    /// Get the AppCode assembly, or throw an exception if it can't be found or compiled.
    /// It can also return null, if there is no code to compile.
    /// </summary>
    /// <param name="spec"></param>
    /// <returns></returns>
    public AssemblyResult TryBuildAppCodeAndLog(HotBuildSpec spec)
    {
        // Add to global history and add specs
        var logSummary = logStore.Add(SxcLogAppCodeLoader, Log);
        logSummary.UpdateSpecs(spec.ToDictionary());

        // Initial message for insights-overview
        var l = Log.Fn<AssemblyResult>($"{spec}", timer: true);

        var assemblyResults = TryBuildAppCode(spec, logSummary);

        // All OK (no errors) - return
        if (string.IsNullOrEmpty(assemblyResults?.ErrorMessages))
            return l.ReturnAsOk(assemblyResults);
        
        // Problems - log and throw
        l.ReturnAsError(null, assemblyResults.ErrorMessages);
        throw new(assemblyResults.ErrorMessages);
    }

    private static readonly ConcurrentDictionary<string, object> CacheKeyLocks = new();

    private AssemblyResult TryBuildAppCode(HotBuildSpec spec, LogStoreEntry logSummary)
    {
        var l = Log.Fn<AssemblyResult>($"{spec}");

        var (result, cacheKey) = assemblyCacheManager.TryGetAppCode(spec);
        logSummary.AddSpec("Cached", $"{result != null} on {cacheKey}");

        if (result != null)
            return l.ReturnAsOk(result);

        var lockObject = CacheKeyLocks.GetOrAdd(cacheKey, _ => new object());

        lock (lockObject)
        {
            // Double check if another thread already built the app code
            (result, cacheKey) = assemblyCacheManager.TryGetAppCode(spec);
            if (result != null)
                return l.ReturnAsOk(result);

            // Get paths
            var (physicalPath, relativePath) = GetAppPaths(AppCodeBase, spec);
            logSummary.AddSpec("PhysicalPath", physicalPath);
            logSummary.AddSpec("RelativePath", relativePath);

            var assemblyResult = appCompilerLazy.Value.GetAppCode(relativePath, spec);

            logSummary.UpdateSpecs(assemblyResult.Infos);

            if (assemblyResult.ErrorMessages.HasValue())
                return l.ReturnAsError(assemblyResult, assemblyResult.ErrorMessages);

            assemblyResult.WatcherFolders = GetWatcherFolders(assemblyResult, spec, physicalPath, Log);
            l.A("Folders to watch:");
            foreach (var watcherFolder in assemblyResult.WatcherFolders)
                l.A($"- '{watcherFolder}'");

            assemblyResult.CacheKey = cacheKey; // used to create cache dependency with CacheEntryChangeMonitor 

            // Triple check if another thread already built the app code
            (result, cacheKey) = assemblyCacheManager.TryGetAppCode(spec);
            if (result != null)
                return l.ReturnAsOk(result);

            // Add compiled assembly to cache
            assemblyCacheManager.Add(
                cacheKey,
                assemblyResult,
                duration: 86400, // 1 day
                changeMonitor: new ChangeMonitor[] { new FolderChangeMonitor(assemblyResult.WatcherFolders) }
            );

            return l.ReturnAsOk(assemblyResult);
        }
    }

    private static IDictionary<string, bool> GetWatcherFolders(AssemblyResult assemblyResult, HotBuildSpec spec, string physicalPathAppCode, ILog log)
    {
        var l = log.Fn<IDictionary<string, bool>>($"{nameof(physicalPathAppCode)}: {physicalPathAppCode}");
        var folders = new Dictionary<string, bool>();

        // take AppCode folder (eg. ...\edition\AppCode)
        IfExistsThenAdd(physicalPathAppCode, true);

        // take parent folder (eg. ...\edition)
        var appCodeParentFolder = Path.GetDirectoryName(physicalPathAppCode);
        if (appCodeParentFolder.IsEmpty()) return l.Return(folders, $"{nameof(appCodeParentFolder)}.IsEmpty");
        IfExistsThenAdd(appCodeParentFolder, false);

        // if no edition was used, then we were already in the root, and should stop now.
        if (spec.Edition.IsEmpty()) return l.Return(folders, $"{nameof(spec.Edition)}.IsEmpty");

        // If we have an edition, and it has an assembly, we don't need to watch the root folder
        if (assemblyResult.HasAssembly) return l.Return(folders, $"{nameof(assemblyResult.HasAssembly)}");

        // If we had an edition and no assembly, then we need to watch the root folder
        // we need to add more folders to watch for cache invalidation

        // App Root folder (eg. ...\)
        var appRootFolder = Path.GetDirectoryName(appCodeParentFolder);
        if (appRootFolder.IsEmpty()) return l.Return(folders, $"{nameof(appRootFolder)}.IsEmpty");

        // Add to watcher list if it exists, otherwise exit, since we can't have subfolders
        if (!IfExistsThenAdd(appRootFolder, false))
            return l.Return(folders, $"{nameof(appRootFolder)} doesn't exist");

        // 
        var appRootAppCode = Path.Combine(appRootFolder, AppCodeBase);
        // Add to watcher list if it exists, otherwise exit, since we can't have subfolders
        if (!IfExistsThenAdd(appRootAppCode, true))
            return l.Return(folders, $"{nameof(appRootAppCode)} doesn't exist");

        // all done
        return l.ReturnAsOk(folders);

        // Helper to add and return info if it exists
        bool IfExistsThenAdd(string folder, bool watchSubfolders)
        {
            var l2 = log.Fn<bool>(folder);
            if (!Directory.Exists(folder)) return l2.ReturnFalse();
            folders.Add(folder, watchSubfolders);
            return l2.ReturnTrue();
        }   
    }

    public (string physicalPath, string relativePath) GetAppPaths(string folder, HotBuildSpec spec)
    {
        var l = Log.Fn<(string physicalPath, string relativePath)>($"{nameof(folder)}: '{folder}'; {spec}");
        l.A($"site id: {site.Id}, ...: {site.AppsRootPhysicalFull}");
        var appPaths = appPathsLazy.Value.Init(site, appStates.GetReader(spec.AppId));
        var folderWithEdition = folder.HasValue() 
            ? (spec.Edition.HasValue() ? Path.Combine(spec.Edition, folder) : folder)
            : spec.Edition;
        var physicalPath = Path.Combine(appPaths.PhysicalPath, folderWithEdition);
        //l.A($"{nameof(physicalPath)}: '{physicalPath}'");
        var relativePath = Path.Combine(appPaths.RelativePath, folderWithEdition);
        //l.A($"{nameof(relativePath)}: '{relativePath}'");
        return l.ReturnAsOk((physicalPath, relativePath));
    }

}