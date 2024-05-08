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
internal class AppCodeCompilerNetCore : AppCodeCompiler
{
    public AppCodeCompilerNetCore(LazySvc<IServerPaths> serverPaths, Generator<Compiler> compiler)
    {
        ConnectSvcs([
            _serverPaths = serverPaths,
            _compiler = compiler
        ]);
        TempAssemblyFolderPath = _serverPaths.Value.FullContentPath(@"App_Data\2sxc.bin");
        // Ensure "2sxc.bin" folder exists to preserve dlls
        Directory.CreateDirectory(TempAssemblyFolderPath);
    }

    private readonly LazySvc<IServerPaths> _serverPaths;
    private readonly Generator<Compiler> _compiler;


    protected internal override AssemblyResult GetAppCode(string virtualPath, HotBuildSpec spec)
    {
        var l = Log.Fn<AssemblyResult>($"{nameof(virtualPath)}: '{virtualPath}'; {spec}", timer: true);

        try
        {
            var sourceFiles = GetSourceFiles(NormalizeFullPath(_serverPaths.Value.FullContentPath(virtualPath.Backslash())));
            if (sourceFiles.Length == 0)
                return l.ReturnAsOk(new());

            var (symbolsPath, assemblyPath) = GetAssemblyLocations(spec);
            var dllName = Path.GetFileName(assemblyPath);
            var assemblyResult = _compiler.New().GetCompiledAssemblyFromFolder(sourceFiles, assemblyPath, symbolsPath, dllName, spec);

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
            var errorMessage = $"Error: Can't compile '{AppCodeDll}' in {Path.GetFileName(virtualPath)}. Details are logged into insights. {ex.Message}";
            return l.ReturnAsError(new(errorMessages: errorMessage), "error");
        }
    }
}