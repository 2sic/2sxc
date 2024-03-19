using System.IO;
using System.Reflection;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Code.Internal.HotBuild;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class DependenciesLoader : ServiceBase
{
    public const string DependenciesFolder = "Dependencies";

    public DependenciesLoader(ILogStore logStore, ISite site, IAppStates appStates, LazySvc<IAppPathsMicroSvc> appPathsLazy, AssemblyCacheManager assemblyCacheManager, LazySvc<AppCodeCompiler> appCodeCompilerLazy) : base("Sys.AppCodeLoad")
    {
        ConnectServices(
            _logStore = logStore,
            _site = site,
            _appStates = appStates,
            _appPathsLazy = appPathsLazy,
            _assemblyCacheManager = assemblyCacheManager,
            _appCodeCompilerLazy = appCodeCompilerLazy
        );
    }
    private readonly ILogStore _logStore;
    private readonly ISite _site;
    private readonly IAppStates _appStates;
    private readonly LazySvc<IAppPathsMicroSvc> _appPathsLazy;
    private readonly AssemblyCacheManager _assemblyCacheManager;
    private readonly LazySvc<AppCodeCompiler> _appCodeCompilerLazy;


    public (List<Assembly> Assemblies, HotBuildSpec Specs) TryGetOrFallback(HotBuildSpec spec)
    {
        var l = Log.Fn<(List<Assembly>, HotBuildSpec)>(spec.ToString());
        var (assemblyResults, cacheKey) = TryGetAssemblyOfDependenciesFromCache(spec, Log);
        if (assemblyResults != null) return l.Return((assemblyResults.Select(r => r.Assembly).ToList(), spec), "Dependencies where cached.");

        var assemblies = LoadDependencyAssembliesOrNull(spec, cacheKey);
        if (assemblies != null) return l.Return((assemblies, spec), $"Dependencies loaded from '/{spec.Edition}'");

        if (spec.Edition.IsEmpty()) return l.Return((null, spec), $"Dependencies not found in '/', done.");

        // try get root edition
        var rootSpec = spec.CloneWithoutEdition();
        var pairFromRoot = TryGetOrFallback(rootSpec);
        return l.Return(pairFromRoot, "Dependencies found in '/'." + (pairFromRoot.Assemblies == null ? ", null." : ""));
    }

    private (List<AssemblyResult> assemblyResults, string cacheKey) TryGetAssemblyOfDependenciesFromCache(HotBuildSpec spec, ILog callerLog)
    {
        var l = callerLog.Fn<(List<AssemblyResult>, string)>($"{spec}");
        var (assemblyResults, cacheKey) = _assemblyCacheManager.TryGetDependencies(spec);
        if (assemblyResults == null) return l.Return((null, cacheKey),"no dependencies in cache");

        l.A($"dependencies from cache: {assemblyResults.Count}");

        foreach (var assemblyResult in assemblyResults)
            l.A(assemblyResult.HasAssembly
                ? $"dependency from cache: {assemblyResult.Assembly.FullName} location: {assemblyResult.Assembly.Location}"
                : $"error in dependency from cache: {assemblyResult.ErrorMessages} ");

        return l.ReturnAsOk((assemblyResults, cacheKey));
    }

    private List<Assembly> LoadDependencyAssembliesOrNull(HotBuildSpec spec, string cacheKey)
    {
        // Add to global history and add specs
        var logSummary = _logStore.Add(SxcLogAppCodeLoader, Log);
        logSummary.UpdateSpecs(spec.ToDictionary());

        // Initial message for insights-overview
        var l = Log.Fn<List<Assembly>>($"{spec}", timer: true);

        var assemblyResults = TryLoadDependencyAssemblies(spec, cacheKey, logSummary);

        return assemblyResults != null 
            ? l.ReturnAsOk(assemblyResults.Select(r => r.Assembly).ToList()) 
            : l.ReturnNull("no dependencies");
    }

    private List<AssemblyResult> TryLoadDependencyAssemblies(HotBuildSpec spec, string cacheKey, LogStoreEntry logSummary)
    {
        var l = Log.Fn<List<AssemblyResult>>($"{spec}");

        // Get paths
        var (physicalPath, relativePath) = GetDependenciesPaths(DependenciesFolder, spec);
        logSummary.AddSpec("Dependencies PhysicalPath", physicalPath);
        logSummary.AddSpec("Dependencies RelativePath", relativePath);

        // missing dependencies folder
        if (!Directory.Exists(physicalPath)) 
            return l.ReturnNull($"{DependenciesFolder} folder do not exists: {physicalPath}");

        var assemblyResults = new List<AssemblyResult>();
        foreach (var dependency in Directory.GetFiles(physicalPath, "*.dll"))
        {
            try
            {
                //var assembly = Assembly.Load(File.ReadAllBytes(dependency));

                var location = _appCodeCompilerLazy.Value.GetDependencyAssemblyLocations(dependency, spec);
                File.Copy(dependency, location, true);
                var assembly = Assembly.LoadFrom(location);
                assemblyResults.Add(new AssemblyResult(assembly, assemblyLocations: [dependency]));
                l.A($"dependency loaded: {assembly.FullName} location: {location}");
            }
            catch (Exception ex)
            {
                // sink
                l.Ex(ex);
            }
        }

        l.A($"dependencies loaded: {assemblyResults.Count}");

        // Add dependency assemblies to cache
        _assemblyCacheManager.Add(cacheKey, assemblyResults, [physicalPath]);
        l.A($"{assemblyResults.Count} dependencies added to cache: {cacheKey}");

        return l.ReturnAsOk(assemblyResults);
    }

    private (string physicalPath, string relativePath) GetDependenciesPaths(string folder, HotBuildSpec spec)
    {
        var l = Log.Fn<(string physicalPath, string relativePath)>($"{spec}");
        var appPaths = _appPathsLazy.Value.Init(_site, _appStates.GetReader(spec.AppId));
        var folderWithEdition = folder.HasValue() 
            ? (spec.Edition.HasValue() ? Path.Combine(spec.Edition, folder) : folder)
            : spec.Edition;
        var physicalPath = Path.Combine(appPaths.PhysicalPath, folderWithEdition);
        l.A($"dependencies {nameof(physicalPath)}: '{physicalPath}'");
        var relativePath = Path.Combine(appPaths.RelativePath, folderWithEdition);
        l.A($"dependencies {nameof(relativePath)}: '{relativePath}'");
        return l.ReturnAsOk((physicalPath, relativePath));
    }
}