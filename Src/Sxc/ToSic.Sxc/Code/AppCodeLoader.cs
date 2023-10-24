using System;
using System.IO;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Paths;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Code
{
    public class AppCodeLoader : ServiceBase, IAppCodeLoader
    {
        public const string AppCodeFolder = "AppCode";

        public AppCodeLoader(ILogStore logStore, ISite site, IAppStates appStates, LazySvc<AppPaths> appPathsLazy, LazySvc<CodeCompiler> codeCompilerLazy) : base("Sys.AppCodeLoad")
        {
            ConnectServices(
                _logStore = logStore,
                _site = site,
                _appStates = appStates,
                _appPathsLazy = appPathsLazy,
                _codeCompilerLazy = codeCompilerLazy
            );
        }
        private readonly ILogStore _logStore;
        private readonly ISite _site;
        private readonly IAppStates _appStates;
        private readonly LazySvc<AppPaths> _appPathsLazy;
        private readonly LazySvc<CodeCompiler> _codeCompilerLazy;

        public bool CompileAppCode(int appId)
        {
            _logStore.Add(Constants.SxcLogAppCodeLoader, Log);

            // Initial message for insights-overview
            var l = Log.Fn<bool>($"{nameof(appId)}: {appId}", timer: true);

            try
            {
                var status = LoadAppCode(appId);

                return l.Return(status, status ? "Ok" : "NoK");
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                return l.ReturnFalse("error");
            }
        }

        private (string physicalPath, string relativePath) GetAppCodeFolderPaths(int appId)
        {
            var appState = _appStates.Get(appId);
            var appPaths = _appPathsLazy.Value.Init(_site, appState);
            var physicalPath = Path.Combine(appPaths.PhysicalPath, AppCodeFolder);
            var relativePath = Path.Combine(appPaths.RelativePath, AppCodeFolder);
            return (physicalPath, relativePath);
        }

        private bool LoadAppCode(int appId)
        {
            var l = Log.Fn<bool>($"{nameof(appId)}: {appId}");

            // Get paths
            var (physicalPath, relativePath) = GetAppCodeFolderPaths(appId);

            l.A($"{nameof(physicalPath)}: '{physicalPath}'; {nameof(relativePath)}: '{relativePath}'");

            if (!Directory.Exists(physicalPath))
                return l.ReturnTrue($"no folder {physicalPath}");

            var compiler = _codeCompilerLazy.Value;

            var dllName = AssemblyName(appId);

            var assemblyResult = compiler.GetAssembly(relativePath, dllName);

            //SaveToDisk(assemblyResult, AssemblyLocation(appId, physicalPath));

            return !assemblyResult.ErrorMessages.HasValue() ? l.ReturnTrue("Ok") : l.ReturnFalse(assemblyResult.ErrorMessages);
        }

        public static string AssemblyName(int appId) => $"AppCode_{appId}.dll";

        private string AssemblyLocation(int appId, string physicalPath = null) =>
            Path.Combine(physicalPath ?? GetAppCodeFolderPaths(appId).physicalPath, AssemblyName(appId));

        private void SaveToDisk(AssemblyResult assemblyResult, string assemblyPath)
        {
            if (assemblyResult.ErrorMessages.HasValue()) return;

            //File.Delete(assemblyPath);
            using (var stream = new FileStream(assemblyPath, FileMode.Create, FileAccess.Write))
            {
                stream.Write(assemblyResult.AssemblyBytes, 0,
                    assemblyResult.AssemblyBytes.Length);
            }


            //var assemblyPart = new CompilationReferencesProvider(assembly);
            //applicationPartManager.ApplicationParts.Add(assemblyPart);

            //var x = peStream.ToArray();
            //// WIP
            //peStream.Seek(0, SeekOrigin.Begin);
            //var metadataReference = MetadataReference.CreateFromStream(peStream);
            ////var moduleMetadata = ModuleMetadata.CreateFromStream(peStream, PEStreamOptions.PrefetchMetadata);
            ////var assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);
            //applicationPartManager.FeatureProviders.Add(new MetadataReferenceFeatureProvider(metadataReference));
        }
    }
}
