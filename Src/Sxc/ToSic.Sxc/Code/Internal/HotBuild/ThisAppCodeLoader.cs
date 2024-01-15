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
public class ThisAppCodeLoader : ServiceBase
{
    public const string ThisAppCodeBase = "ThisApp";
    public const string ThisAppBinFolder = "bin";

    public ThisAppCodeLoader(ILogStore logStore, ISite site, IAppStates appStates, LazySvc<IAppPathsMicroSvc> appPathsLazy, LazySvc<ThisAppCodeCompiler> thisAppCodeCompilerLazy, AssemblyCacheManager assemblyCacheManager) : base("Sys.AppCodeLoad")
    {
        ConnectServices(
            _logStore = logStore,
            _site = site,
            _appStates = appStates,
            _appPathsLazy = appPathsLazy,
            _thisAppCodeCompilerLazy = thisAppCodeCompilerLazy,
            _assemblyCacheManager = assemblyCacheManager
        );
    }
    private readonly ILogStore _logStore;
    private readonly ISite _site;
    private readonly IAppStates _appStates;
    private readonly LazySvc<IAppPathsMicroSvc> _appPathsLazy;
    private readonly LazySvc<ThisAppCodeCompiler> _thisAppCodeCompilerLazy;
    private readonly AssemblyCacheManager _assemblyCacheManager;


    public (Assembly, HotBuildSpec) TryGetOrFallback(HotBuildSpec spec)
    {
        var assembly = TryGetAssemblyOfCodeFromCache(spec, Log)?.Assembly;
        if (assembly != null) return (assembly, spec);

        assembly = GetAppCodeAssemblyOrThrow(spec);
        if (assembly != null) return (assembly, spec);

        return spec.Edition.IsEmpty()
            ? (null, spec) 
            : TryGetOrFallback(spec.CloneWithoutEdition()); // fallback to non-edition in root of app
    }

    public static AssemblyResult TryGetAssemblyOfCodeFromCache(HotBuildSpec spec, ILog callerLog)
    {
        var l = callerLog.Fn<AssemblyResult>($"{spec}");

        var (result, _) = AssemblyCacheManager.TryGetThisAppCode(spec);
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

        var assemblyResults = TryLoadAppCodeAssembly(spec, logSummary);

        if (string.IsNullOrEmpty(assemblyResults?.ErrorMessages))
            return l.ReturnAsOk(assemblyResults?.Assembly);
        
        l.ReturnAsError(null, assemblyResults.ErrorMessages);
        throw new Exception(assemblyResults.ErrorMessages);
    }

    private AssemblyResult TryLoadAppCodeAssembly(HotBuildSpec spec, LogStoreEntry logSummary)
    {
        var l = Log.Fn<AssemblyResult>($"{spec}");

        var (result, cacheKey) = AssemblyCacheManager.TryGetThisAppCode(spec);
        logSummary.AddSpec("Cached", $"{result != null} on {cacheKey}");

        if (result != null)
            return l.ReturnAsOk(result);

        // Get paths
        var (physicalPath, relativePath) = GetAppPaths(ThisAppCodeBase + "\\" + spec.Segment, spec);
        l.A($"{nameof(physicalPath)}: '{physicalPath}'; {nameof(relativePath)}: '{relativePath}'");
        logSummary.AddSpec("PhysicalPath", physicalPath);
        logSummary.AddSpec("RelativePath", relativePath);

        if (!Directory.Exists(physicalPath))
            return l.ReturnAsError(null, $"no folder {physicalPath}");

        var assemblyResult = _thisAppCodeCompilerLazy.Value.GetAppCode(relativePath, spec);

        logSummary.UpdateSpecs(assemblyResult.Infos);

        if (assemblyResult.ErrorMessages.HasValue())
            return l.ReturnAsError(assemblyResult, assemblyResult.ErrorMessages);

        assemblyResult.WatcherFolders = new[] { physicalPath };

        var (refsAssemblyPath, _) = GetAppPaths(ThisAppBinFolder, spec);
        CopyAssemblyForRefs(assemblyResult.AssemblyLocations[1], Path.Combine(refsAssemblyPath, ThisAppCodeCompiler.ThisAppCodeDll));

        // Add compiled assembly to cache
        _assemblyCacheManager.Add(
            cacheKey,
            assemblyResult,
            changeMonitor: new ChangeMonitor[] { new FolderChangeMonitor(assemblyResult.WatcherFolders) },
            updateCallback: _ => AssembliesDelete(assemblyResult.AssemblyLocations.Append(refsAssemblyPath))
        );

        return l.ReturnAsOk(assemblyResult);
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

    private static void AssembliesDelete(IEnumerable<string> list)
    {
        if (list == null) return;
        foreach (var assembly in list)
        {
            try
            {
                File.Delete(assembly);
            }
            catch
            {
                // ignore
            }
        }
    }

    private static void CopyAssemblyForRefs(string source, string destination)
    {
        if (!File.Exists(source)) return;

        var destinationFolder = Path.GetDirectoryName(destination);
        if (destinationFolder != null) Directory.CreateDirectory(destinationFolder);

        File.Copy(source, destination, true);
    }
}