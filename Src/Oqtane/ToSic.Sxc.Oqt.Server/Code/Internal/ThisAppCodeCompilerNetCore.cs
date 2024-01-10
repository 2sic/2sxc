using System;
using System.IO;
using ToSic.Eav.Helpers;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code.Internal.HotBuild;

namespace ToSic.Sxc.Oqt.Server.Code.Internal;

[PrivateApi]
internal class ThisAppCodeCompilerNetCore : ThisAppCodeCompiler
{
    public ThisAppCodeCompilerNetCore(LazySvc<IServerPaths> serverPaths, LazySvc<ThisAppCodeLoader> thisAppCodeLoader)
    {
        ConnectServices(
            _serverPaths = serverPaths,
            _thisAppCodeLoader = thisAppCodeLoader
        );
    }

    private readonly LazySvc<IServerPaths> _serverPaths;
    private readonly LazySvc<ThisAppCodeLoader> _thisAppCodeLoader;

    protected internal override AssemblyResult GetAppCode(string virtualPath, HotBuildSpec spec)
    {
        var l = Log.Fn<AssemblyResult>($"{nameof(virtualPath)}: '{virtualPath}'; {nameof(spec.AppId)}: {spec.AppId}; {nameof(spec.Edition)}: '{spec.Edition}'", timer: true);

        try
        {
            var (sourceFiles, errResult) = GetSourceFilesOrError(NormalizeFullPath(_serverPaths.Value.FullContentPath(virtualPath.Backslash())));

            if (errResult != null)
                return l.ReturnAsError(errResult, errResult.ErrorMessages);

            var assemblyLocations = GetAssemblyLocations(spec);
            var dllName = Path.GetFileName(assemblyLocations[1]);
            var assemblyResult = new Compiler(_thisAppCodeLoader).GetCompiledAssemblyFromFolder(sourceFiles, assemblyLocations[1], assemblyLocations[0], dllName);

            // Compile ok
            if (assemblyResult.ErrorMessages.IsEmpty())
            {
                LogAllTypes(assemblyResult.Assembly);
                return l.ReturnAsOk(new AssemblyResult(assembly: assemblyResult.Assembly, assemblyLocations: assemblyLocations));
            }

            return l.ReturnAsError(new AssemblyResult(errorMessages: assemblyResult.ErrorMessages), assemblyResult.ErrorMessages);
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            var errorMessage = $"Error: Can't compile '{ThisAppCodeDll}' in {Path.GetFileName(virtualPath)}. Details are logged into insights. {ex.Message}";
            return l.ReturnAsError(new AssemblyResult(errorMessages: errorMessage), "error");
        }
    }

    private string[] GetAssemblyLocations(HotBuildSpec spec)
    {
        var l = Log.Fn<string[]>($"{nameof(spec.AppId)}: {spec.AppId}; {nameof(spec.Edition)}: '{spec.Edition}'");
        var tempAssemblyFolderPath = _serverPaths.Value.FullContentPath(@"App_Data\2sxc.bin");
        l.A($"TempAssemblyFolderPath: '{tempAssemblyFolderPath}'");
        // Ensure "2sxc.bin" folder exists to preserve dlls
        Directory.CreateDirectory(tempAssemblyFolderPath);

        // need name 
        var assemblyName = GetAppCodeDllName(tempAssemblyFolderPath, spec);
        l.A($"AssemblyName: '{assemblyName}'");
        var assemblyFilePath = Path.Combine(tempAssemblyFolderPath, $"{assemblyName}.dll");
        l.A($"AssemblyFilePath: '{assemblyFilePath}'");
        var symbolsFilePath = Path.Combine(tempAssemblyFolderPath, $"{assemblyName}.pdb");
        l.A($"SymbolsFilePath: '{symbolsFilePath}'");
        var assemblyLocations = new[] { symbolsFilePath, assemblyFilePath };
        return l.ReturnAsOk(assemblyLocations);
    }
}