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
public class AppCodeLoader : ServiceBase
{
    public const string AppCodeBase = "AppCode";
    //public const string AppCodeBinFolder = "bin";
    private const string AppRoot = HotBuildSpec.AppRoot;

    public AppCodeLoader(ILogStore logStore, ISite site, IAppStates appStates, LazySvc<IAppPathsMicroSvc> appPathsLazy, LazySvc<AppCodeCompiler> appCompilerLazy, AssemblyCacheManager assemblyCacheManager) : base("Sys.AppCodeLoad")
    {
        ConnectServices(
            _logStore = logStore,
            _site = site,
            _appStates = appStates,
            _appPathsLazy = appPathsLazy,
            _appCompilerLazy = appCompilerLazy,
            _assemblyCacheManager = assemblyCacheManager
        );
    }
    private readonly ILogStore _logStore;
    private readonly ISite _site;
    private readonly IAppStates _appStates;
    private readonly LazySvc<IAppPathsMicroSvc> _appPathsLazy;
    private readonly LazySvc<AppCodeCompiler> _appCompilerLazy;
    private readonly AssemblyCacheManager _assemblyCacheManager;


    public (Assembly Assembly, HotBuildSpec Specs) TryGetOrFallback(HotBuildSpec spec)
    {
        var l = Log.Fn<(Assembly, HotBuildSpec)>(spec.ToString());
        var assembly = TryGetAssemblyOfAppCodeFromCache(spec, Log)?.Assembly;
        if (assembly != null) return l.Return((assembly, spec), "AppCode assembly was cached.");

        assembly = GetAppCodeAssemblyOrThrow(spec);
        if (assembly != null) return l.Return((assembly, spec), $"AppCode assembly compiled in '{(spec.Edition.IsEmpty() ? AppRoot : spec.Edition)}'.");

        if (spec.Edition.IsEmpty()) return l.Return((null, spec), $"AppCode not found in '{AppRoot}', done.");

        // try get root edition
        var rootSpec = spec.CloneWithoutEdition();
        var pairFromRoot = TryGetOrFallback(rootSpec);
        return l.Return(pairFromRoot, pairFromRoot.Assembly == null ? $"AppCode not found in '{AppRoot}', null." : $"AppCode found in '{AppRoot}'.");
    }

    public static AssemblyResult TryGetAssemblyOfAppCodeFromCache(HotBuildSpec spec, ILog callerLog)
    {
        var l = callerLog.Fn<AssemblyResult>($"{spec}");

        var (result, _) = AssemblyCacheManager.TryGetAppCode(spec);
        return result != null
            ? l.ReturnAsOk(result)
            : l.ReturnNull();
    }

    public Assembly GetAppCodeAssemblyOrThrow(HotBuildSpec spec)
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

        var (result, cacheKey) = AssemblyCacheManager.TryGetAppCode(spec);
        logSummary.AddSpec("Cached", $"{result != null} on {cacheKey}");

        if (result != null)
            return l.ReturnAsOk(result);

        // Get paths
        var (physicalPath, relativePath) = GetAppPaths(AppCodeBase, spec);
        //l.A($"{nameof(physicalPath)}: '{physicalPath}'; {nameof(relativePath)}: '{relativePath}'");
        logSummary.AddSpec("PhysicalPath", physicalPath);
        logSummary.AddSpec("RelativePath", relativePath);
 
        var assemblyResult = _appCompilerLazy.Value.GetAppCode(relativePath, spec);

        logSummary.UpdateSpecs(assemblyResult.Infos);

        if (assemblyResult.ErrorMessages.HasValue())
            return l.ReturnAsError(assemblyResult, assemblyResult.ErrorMessages);

        assemblyResult.WatcherFolders = GetWatcherFolders(assemblyResult, spec, physicalPath, Log);
        l.A("Folders to watch:");
        foreach (var watcherFolder in assemblyResult.WatcherFolders)
            l.A($"- '{watcherFolder}'");

        // Idea: put dll in the App/bin folder, for VS Intellisense - ATM not relevant
        //var (refsAssemblyPath, _) = GetAppPaths(AppCodeBinFolder, spec);
        //if (assemblyResult.HasAssembly)
        //    CopyAssemblyForRefs(assemblyResult.AssemblyLocations[1], Path.Combine(refsAssemblyPath, AppCodeCompiler.AppCodeDll));

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

    private static IDictionary<string, bool> GetWatcherFolders(AssemblyResult assemblyResult, HotBuildSpec spec, string physicalPathAppCode, ILog log)
    {
        var l = log.Fn<IDictionary<string, bool>>($"{nameof(physicalPathAppCode)}: {physicalPathAppCode}");
        var folders = new Dictionary<string, bool>();

        // take AppCode folder (eg. ...\edition\AppCode)
        IfExistsThenAdd(physicalPathAppCode, true);

        // take parent folder (eg. ...\edition)
        var appCodeParentFolder = Path.GetDirectoryName(physicalPathAppCode);
        if (appCodeParentFolder.IsEmpty()) return l.Return(folders, $"exit {nameof(appCodeParentFolder)}");
        IfExistsThenAdd(appCodeParentFolder, false);

        // if no edition was used, then we were already in the root, and should stop now.
        if (spec.Edition.IsEmpty()) return l.Return(folders, $"exit {nameof(spec.Edition)}");

        // If we have an edition, and it has an assembly, we don't need to watch the root folder
        if (assemblyResult.HasAssembly) return l.Return(folders, $"exit {nameof(assemblyResult.HasAssembly)}");

        // If we had an edition and no assembly, then we need to watch the root folder
        // we need to add more folders to watch for cache invalidation

        // App Root folder (eg. ...\)
        var appRootFolder = Path.GetDirectoryName(appCodeParentFolder);
        if (appRootFolder.IsEmpty()) return l.Return(folders, $"exit {nameof(appRootFolder)}.IsEmpty");
        // Add to watcher list if it exists, otherwise exit, since we can't have subfolders
        if (!IfExistsThenAdd(appRootFolder, false)) return l.Return(folders, $"{nameof(appRootFolder)}");

        // 
        var appRootAppCode = Path.Combine(appRootFolder, AppCodeBase);
        // Add to watcher list if it exists, otherwise exit, since we can't have subfolders
        if (!IfExistsThenAdd(appRootAppCode, true)) return l.Return(folders, $"{nameof(appRootAppCode)}");

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
        l.A($"site id: {_site.Id}, ...: {_site.AppsRootPhysicalFull}");
        var appPaths = _appPathsLazy.Value.Init(_site, _appStates.GetReader(spec.AppId));
        var folderWithEdition = folder.HasValue() 
            ? (spec.Edition.HasValue() ? Path.Combine(spec.Edition, folder) : folder)
            : spec.Edition;
        var physicalPath = Path.Combine(appPaths.PhysicalPath, folderWithEdition);
        //l.A($"{nameof(physicalPath)}: '{physicalPath}'");
        var relativePath = Path.Combine(appPaths.RelativePath, folderWithEdition);
        //l.A($"{nameof(relativePath)}: '{relativePath}'");
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