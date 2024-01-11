using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using System.CodeDom.Compiler;
using System.IO;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Dnn.Compile;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Code;

[PrivateApi]
internal class ThisAppCodeCompilerNetFull : ThisAppCodeCompiler
{

    private readonly IHostingEnvironmentWrapper _hostingEnvironment;
    private readonly IReferencedAssembliesProvider _referencedAssembliesProvider;

    public ThisAppCodeCompilerNetFull(IHostingEnvironmentWrapper hostingEnvironment, IReferencedAssembliesProvider referencedAssembliesProvider)
    {
        ConnectServices(
            _hostingEnvironment = hostingEnvironment,
            _referencedAssembliesProvider = referencedAssembliesProvider
        );
    }

    protected internal override AssemblyResult GetAppCode(string relativePath, HotBuildSpec spec)
    {
        var l = Log.Fn<AssemblyResult>($"{nameof(relativePath)}: '{relativePath}'; {spec}");

        try
        {
            // Get all C# files in the folder
            var (sourceFiles, errorResult) = GetSourceFilesOrError(NormalizeFullPath(_hostingEnvironment.MapPath(relativePath)));
            if (errorResult != null)
                return l.ReturnAsError(errorResult, errorResult.ErrorMessages);

            var (symbolsPath, assemblyPath) = GetAssemblyLocations(spec);

            var results = GetCompiledAssemblyFromFolder(sourceFiles, assemblyPath);

            var dicInfos = new Dictionary<string, string>
            {
                //["DllName"] = dllName,
                ["Files"] = sourceFiles.Length.ToString(),
                ["Errors"] = results.Errors.HasErrors.ToString(),
                // ["Assembly"] = assemblyResult.Assembly?.FullName ?? "null",
                ["AssemblyPath"] = assemblyPath,
                ["SymbolsPath"] = symbolsPath,
            };

            // Compile ok
            if (!results.Errors.HasErrors)
            {
                LogAllTypes(results.CompiledAssembly);
                return l.ReturnAsOk(new(assembly: results.CompiledAssembly, assemblyLocations: [symbolsPath, assemblyPath ], infos: dicInfos));
            }

            // Compile error case
            var errors = "";
            foreach (CompilerError error in results.Errors)
            {
                var msg = $"{(error.IsWarning ? "Warning" : "Error")} ({error.ErrorNumber}): {error.ErrorText} in '{error.FileName}' (Line: {error.Line}, Column: {error.Column}).";
                l.E(msg);
                errors += $"{msg}\n";
            }

            return l.ReturnAsError(new(errorMessages: errors, infos: dicInfos), errors);
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            var errorMessage = $"Error: Can't compile '{ThisAppCodeDll}' in {Path.GetFileName(relativePath)}. Details are logged into insights. {ex.Message}";
            return l.ReturnAsError(new(errorMessages: errorMessage));
        }
    }

    private (string SymbolsPath, string AssemblyPath) GetAssemblyLocations(HotBuildSpec spec)
    {
        var l = Log.Fn<(string, string)>($"{nameof(spec.AppId)}: {spec.AppId}; {nameof(spec.Edition)}: '{spec.Edition}'");
        var tempAssemblyFolderPath = Path.Combine(_hostingEnvironment.MapPath("~/App_Data"), "2sxc.bin");
        l.A($"TempAssemblyFolderPath: '{tempAssemblyFolderPath}'");
        // Ensure "2sxc.bin" folder exists to preserve dlls
        Directory.CreateDirectory(tempAssemblyFolderPath);

        // need random name, because assemblies has to be preserved on disk, and we can not replace them until AppDomain is unloaded 
        var assemblyName = GetAppCodeDllName(tempAssemblyFolderPath, spec);
        l.A($"AssemblyName: '{assemblyName}'");
        var assemblyFilePath = Path.Combine(tempAssemblyFolderPath, $"{assemblyName}.dll");
        l.A($"AssemblyFilePath: '{assemblyFilePath}'");
        var symbolsFilePath = Path.Combine(tempAssemblyFolderPath, $"{assemblyName}.pdb");
        l.A($"SymbolsFilePath: '{symbolsFilePath}'");
        var assemblyLocations = (symbolsFilePath, assemblyFilePath);
        return l.ReturnAsOk(assemblyLocations);
    }

    private CompilerResults GetCompiledAssemblyFromFolder(string[] sourceFiles, string assemblyFilePath)
    {
        var l = Log.Fn<CompilerResults>($"{nameof(sourceFiles)}: {sourceFiles.Length}; {nameof(assemblyFilePath)}: '{assemblyFilePath}'", timer: true);

        // need to save dll so latter can be loaded by roslyn compiler
        var parameters = new CompilerParameters(null, assemblyFilePath)
            {
                GenerateInMemory = false,
                GenerateExecutable = false,
                IncludeDebugInformation = true,
                CompilerOptions = $"/optimize- /warnaserror- {DnnRoslynConstants.CompilerOptionLanguageVersion} {DnnRoslynConstants.DefaultDisableWarnings}",
            };

        // Add all referenced assemblies
        parameters.ReferencedAssemblies.AddRange(_referencedAssembliesProvider.Locations());

        var codeProvider = new CSharpCodeProvider();
        var compilerResults = codeProvider.CompileAssemblyFromFile(parameters, sourceFiles);

        return compilerResults.Errors.HasErrors
            ? l.ReturnAsError(compilerResults)
            : l.ReturnAsOk(compilerResults);
    }

}