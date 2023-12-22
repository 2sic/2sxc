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

namespace ToSic.Sxc.Code
{
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

        public static AssemblyResult TryGetAssemblyOfCodeFromCache(int appId, ILog callerLog)
        {
            var l = callerLog.Fn<AssemblyResult>($"{nameof(appId)}: {appId}");

            var (result, _) = AssemblyCacheManager.TryGetThisAppCode(appId);
            return result != null
                ? l.ReturnAsOk(result)
                : l.ReturnNull();
        }

        public Assembly GetAppCodeAssemblyOrNull(int appId)
        {
            _logStore.Add(Constants.SxcLogAppCodeLoader, Log);

            // Initial message for insights-overview
            var l = Log.Fn<Assembly>($"{nameof(appId)}: {appId}", timer: true);

            try
            {
                var assemblyResults = TryLoadAppCodeAssembly(appId);
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

        private AssemblyResult TryLoadAppCodeAssembly(int appId)
        {
            var l = Log.Fn<AssemblyResult>($"{nameof(appId)}: {appId}");

            var (result, cacheKey) = AssemblyCacheManager.TryGetThisAppCode(appId);
            if (result != null)
                return l.ReturnAsOk(result);

            // Get paths
            var (physicalPath, relativePath) = GetAppPaths(appId, ThisAppCodeFolder);
            l.A($"{nameof(physicalPath)}: '{physicalPath}'; {nameof(relativePath)}: '{relativePath}'");

            if (!Directory.Exists(physicalPath))
                return l.ReturnAsError(null, $"no folder {physicalPath}");

            var assemblyResult = _thisAppCodeCompilerLazy.Value.GetAppCode(relativePath, appId);

            if (assemblyResult.ErrorMessages.HasValue()) 
                return l.ReturnAsError(assemblyResult, assemblyResult.ErrorMessages);

            assemblyResult.WatcherFolders = new[] { physicalPath };

            var (refsAssemblyPath, _) = GetAppPaths(appId, ThisAppBinFolder);
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

        private (string physicalPath, string relativePath) GetAppPaths(int appId, string folder)
        {
            var l = Log.Fn<(string physicalPath, string relativePath)>($"{nameof(appId)}: {appId}; {nameof(folder)}:'{folder}'");
            var appPaths = _appPathsLazy.Value.Init(_site, _appStates.GetReader(appId));
            var physicalPath = Path.Combine(appPaths.PhysicalPath, folder);
            l.A($"{nameof(physicalPath)}: '{physicalPath}'");
            var relativePath = Path.Combine(appPaths.RelativePath, folder);
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
}
