using System;
using System.Collections.Generic;
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
internal class ThisAppCompilerNetCore : ThisAppCompiler
{
    public ThisAppCompilerNetCore(LazySvc<IServerPaths> serverPaths, LazySvc<ThisAppLoader> thisAppCodeLoader)
    {
        ConnectServices(
            _serverPaths = serverPaths,
            _thisAppCodeLoader = thisAppCodeLoader
        );
    }

    private readonly LazySvc<IServerPaths> _serverPaths;
    private readonly LazySvc<ThisAppLoader> _thisAppCodeLoader;

    protected internal override AssemblyResult GetThisApp(string virtualPath, HotBuildSpec spec)
    {
        var l = Log.Fn<AssemblyResult>($"{nameof(virtualPath)}: '{virtualPath}'; {spec}", timer: true);

        try
        {
            var sourceFiles = GetSourceFiles(NormalizeFullPath(_serverPaths.Value.FullContentPath(virtualPath.Backslash())));
            if (sourceFiles.Length == 0)
                return l.ReturnAsOk(new());

            var (symbolsPath, assemblyPath) = GetAssemblyLocations(spec);
            var dllName = Path.GetFileName(assemblyPath);
            var assemblyResult = new Compiler(_thisAppCodeLoader).GetCompiledAssemblyFromFolder(sourceFiles, assemblyPath, symbolsPath, dllName);

            var dicInfos = new Dictionary<string, string>
            {
                ["DllName"] = dllName,
                ["Files"] = sourceFiles.Length.ToString(),
                ["Errors"] = assemblyResult.ErrorMessages?.Length.ToString(),
                ["Assembly"] = assemblyResult.Assembly?.FullName ?? "null",
                ["AssemblyPath"] = assemblyPath,
                ["SymbolsPath"] = symbolsPath,
            };

            // Compile ok
            if (assemblyResult.ErrorMessages.IsEmpty())
            {
                LogAllTypes(assemblyResult.Assembly);
                return l.ReturnAsOk(new(assembly: assemblyResult.Assembly, assemblyLocations: [symbolsPath, assemblyPath], infos: dicInfos));
            }

            return l.ReturnAsError(new(errorMessages: assemblyResult.ErrorMessages, infos: dicInfos), assemblyResult.ErrorMessages);
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            var errorMessage = $"Error: Can't compile '{ThisAppDll}' in {Path.GetFileName(virtualPath)}. Details are logged into insights. {ex.Message}";
            return l.ReturnAsError(new(errorMessages: errorMessage), "error");
        }
    }

    private (string SymbolsPath, string AssemblyPath) GetAssemblyLocations(HotBuildSpec spec)
    {
        var l = Log.Fn<(string, string)>($"{spec}");
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
        var assemblyLocations = ( symbolsFilePath, assemblyFilePath );
        return l.ReturnAsOk(assemblyLocations);
    }
}