using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Paths;
using ToSic.Eav.Caching.CachingMonitors;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Code.Internal.HotBuild;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ThisAppCodeLoader : ServiceBase
{
    public const string ThisAppCodeFolder = "ThisApp\\Code";
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

    public static AssemblyResult TryGetAssemblyOfCodeFromCache(HotBuildSpec spec, ILog callerLog)
    {
        var l = callerLog.Fn<AssemblyResult>($"{nameof(spec.AppId)}: {spec.AppId}; {nameof(spec.Edition)}: '{spec.Edition}'");

        var (result, _) = AssemblyCacheManager.TryGetThisAppCode(spec);
        return result != null
            ? l.ReturnAsOk(result)
            : l.ReturnNull();
    }

    public Assembly GetAppCodeAssemblyOrNull(HotBuildSpec spec)
    {
        _logStore.Add(SxcLogging.SxcLogAppCodeLoader, Log);

        // Initial message for insights-overview
        var l = Log.Fn<Assembly>($"{nameof(spec.AppId)}: {spec.AppId}; {nameof(spec.Edition)}: '{spec.Edition}'", timer: true);

        try
        {
            var assemblyResults = TryLoadAppCodeAssembly(spec);
            return string.IsNullOrEmpty(assemblyResults?.ErrorMessages)
                ? l.ReturnAsOk(assemblyResults?.Assembly)
                : l.ReturnAsError(assemblyResults?.Assembly);
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnAsError(null);
        }
    }

    private AssemblyResult TryLoadAppCodeAssembly(HotBuildSpec spec)
    {
        var l = Log.Fn<AssemblyResult>($"{nameof(spec.AppId)}: {spec.AppId}; {nameof(spec.Edition)}: '{spec.Edition}'");

        var (result, cacheKey) = AssemblyCacheManager.TryGetThisAppCode(spec);
        if (result != null)
            return l.ReturnAsOk(result);

        // Get paths
        var (physicalPath, relativePath) = GetAppPaths(ThisAppCodeFolder, spec);
        l.A($"{nameof(physicalPath)}: '{physicalPath}'; {nameof(relativePath)}: '{relativePath}'");

        if (!Directory.Exists(physicalPath))
            return l.ReturnAsError(null, $"no folder {physicalPath}");

        var assemblyResult = _thisAppCodeCompilerLazy.Value.GetAppCode(relativePath, spec);

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

    private (string physicalPath, string relativePath) GetAppPaths(string folder, HotBuildSpec spec)
    {
        var l = Log.Fn<(string physicalPath, string relativePath)>($"{nameof(spec.AppId)}: {spec.AppId}; {nameof(folder)}:'{folder}'; {nameof(spec.Edition)}: '{spec.Edition}'");
        var appPaths = _appPathsLazy.Value.Init(_site, _appStates.GetReader(spec.AppId));
        var folderWithEdition = spec.Edition.HasValue() ? Path.Combine(spec.Edition, folder) : folder;
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