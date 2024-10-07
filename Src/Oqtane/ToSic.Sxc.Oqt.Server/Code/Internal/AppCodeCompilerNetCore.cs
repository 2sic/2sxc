using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ToSic.Eav.Helpers;
using ToSic.Eav.Internal.Configuration;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code.Internal.HotBuild;

namespace ToSic.Sxc.Oqt.Server.Code.Internal;

[PrivateApi]
internal class AppCodeCompilerNetCore(LazySvc<IServerPaths> serverPaths, Generator<Compiler> compiler, IGlobalConfiguration globalConfiguration, SourceCodeHasher sourceCodeHasher) : AppCodeCompiler(globalConfiguration, sourceCodeHasher, connect: [serverPaths, compiler, sourceCodeHasher])
{
    protected internal override AssemblyResult GetAppCode(string virtualPath, HotBuildSpecWithSharedSuffix spec)
    {
        var l = Log.Fn<AssemblyResult>($"{nameof(virtualPath)}: '{virtualPath}'; {spec}", timer: true);

        try
        {
            var sourceRootPath = NormalizeFullPath(serverPaths.Value.FullContentPath(virtualPath.Backslash()));
            var sourceFiles = GetSourceFiles(sourceRootPath);
            if (sourceFiles.Length == 0)
                return l.ReturnAsOk(new());

            var (symbolsPath, assemblyPath) = GetAssemblyLocations(spec, sourceRootPath);
            var dllName = Path.GetFileName(assemblyPath);
            var assemblyResult = File.Exists(assemblyPath) 
                ? new AssemblyResult(assembly: new SimpleUnloadableAssemblyLoadContext().LoadFromAssemblyPath(assemblyPath))
                : compiler.New().GetCompiledAssemblyFromFolder(sourceFiles, assemblyPath, symbolsPath, dllName, spec);

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