using System;
using System.IO;
using System.Reflection;
using System.Runtime.Caching;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Paths;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using IAppCodeLoader = ToSic.Sxc.Apps.IAppCodeLoader;

namespace ToSic.Sxc.Code
{
    public class AppCodeLoader : ServiceBase, IAppCodeLoader
    {
        public const string AppCodeFolder = "AppCode";

        public AppCodeLoader(ILogStore logStore, ISite site, IAppStates appStates, LazySvc<AppPaths> appPathsLazy, LazySvc<CodeCompiler> codeCompilerLazy, AssemblyCacheManager assemblyCacheManager) : base("Sys.AppCodeLoad")
        {
            ConnectServices(
                _logStore = logStore,
                _site = site,
                _appStates = appStates,
                _appPathsLazy = appPathsLazy,
                _codeCompilerLazy = codeCompilerLazy,
                _assemblyCacheManager = assemblyCacheManager
            );
        }
        private readonly ILogStore _logStore;
        private readonly ISite _site;
        private readonly IAppStates _appStates;
        private readonly LazySvc<AppPaths> _appPathsLazy;
        private readonly LazySvc<CodeCompiler> _codeCompilerLazy;
        private readonly AssemblyCacheManager _assemblyCacheManager;

        public Assembly GetAppCodeAssemblyOrNull(int appId)
        {
            _logStore.Add(Constants.SxcLogAppCodeLoader, Log);

            // Initial message for insights-overview
            var l = Log.Fn<Assembly>($"{nameof(appId)}: {appId}", timer: true);

            try
            {
                var assemblyResults = GetAppCodeAssemblyResultsCachedOrCreate(appId);
                return l.Return(assemblyResults?.Assembly, assemblyResults?.ErrorMessages != null ? "Ok" : "NoK");
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                return l.Return(null, "error");
            }
        }

        private (string physicalPath, string relativePath) GetAppCodeFolderPaths(int appId)
        {
            var appState = _appStates.Get(appId);
            var appPaths = _appPathsLazy.Value.Init(_site, appState);
            var physicalPath = Path.Combine(appPaths.PhysicalPath, AppCodeFolder);
            var relativePath = Path.Combine(appPaths.RelativePath, AppCodeFolder);
            // get App_Data physical path

            return (physicalPath, relativePath);
        }

        private AssemblyResult GetAppCodeAssemblyResultsCachedOrCreate(int appId)
        {
            var l = Log.Fn<AssemblyResult>($"{nameof(appId)}: {appId}");

            string cacheKey = _assemblyCacheManager.KeyAppCode(appId);
            if (_assemblyCacheManager.Has(cacheKey))
                return l.Return(_assemblyCacheManager.Get(cacheKey), "Ok");

            // Get paths
            var (physicalPath, relativePath) = GetAppCodeFolderPaths(appId);

            l.A($"{nameof(physicalPath)}: '{physicalPath}'; {nameof(relativePath)}: '{relativePath}'");

            if (!Directory.Exists(physicalPath))
                return l.Return(null, $"no folder {physicalPath}");

            var compiler = _codeCompilerLazy.Value;

            var dllName = AssemblyName(appId);

            var assemblyResult = compiler.GetAssembly(relativePath, dllName);

            if (!assemblyResult.ErrorMessages.HasValue())
            {
                _assemblyCacheManager.Add(cacheKey, assemblyResult, appPaths: new[] { physicalPath }, updateCallback: UpdateCallback);
                // SaveToDisk(assemblyResult, AssemblyLocation(appId, physicalPath));
                return l.Return(assemblyResult, "Ok");

                void UpdateCallback(CacheEntryUpdateArguments arguments) => AssembliesDelete(assemblyResult.AssemblyLocations);
            }

            return l.Return(assemblyResult, assemblyResult.ErrorMessages);
        }

        public static void AssembliesDelete(string[] list)
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

        public static string AssemblyName(int appId) => $"AppCode_{appId}.dll";

        //private string AssemblyLocation(int appId, string physicalPath = null) =>
        //    Path.Combine(physicalPath ?? GetAppCodeFolderPaths(appId).physicalPath, AssemblyName(appId));

        //private void SaveToDisk(AssemblyResult assemblyResult, string assemblyPath)
        //{
        //    if (assemblyResult.ErrorMessages.HasValue()) return;

        //    //File.Delete(assemblyPath);
        //    using (var stream = new FileStream(assemblyPath, FileMode.Create, FileAccess.Write))
        //    {
        //        stream.Write(assemblyResult.AssemblyBytes, 0,
        //            assemblyResult.AssemblyBytes.Length);
        //    }

        //    //var assemblyPart = new CompilationReferencesProvider(assembly);
        //    //applicationPartManager.ApplicationParts.Add(assemblyPart);

        //    //var x = peStream.ToArray();
        //    //// WIP
        //    //peStream.Seek(0, SeekOrigin.Begin);
        //    //var metadataReference = MetadataReference.CreateFromStream(peStream);
        //    ////var moduleMetadata = ModuleMetadata.CreateFromStream(peStream, PEStreamOptions.PrefetchMetadata);
        //    ////var assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);
        //    //applicationPartManager.FeatureProviders.Add(new MetadataReferenceFeatureProvider(metadataReference));
        //}
    }
}
