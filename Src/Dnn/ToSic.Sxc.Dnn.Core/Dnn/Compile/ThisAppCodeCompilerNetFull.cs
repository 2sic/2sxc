using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using System;
using System.CodeDom.Compiler;
using System.IO;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code.Internal;
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

    protected internal override AssemblyResult GetAppCode(string relativePath, int appId = 0)
    {
        var l = Log.Fn<AssemblyResult>($"{nameof(relativePath)}: '{relativePath}'; {nameof(appId)}: {appId}");

        try
        {

            // Get all C# files in the folder
            var (sourceFiles, errorResult) = GetSourceFilesOrError(NormalizeFullPath(_hostingEnvironment.MapPath(relativePath)));
            if (errorResult != null)
                return l.ReturnAsError(errorResult, errorResult.ErrorMessages);

            var assemblyLocations = GetAssemblyLocations(appId);

            var results = GetCompiledAssemblyFromFolder(sourceFiles, assemblyLocations[1]);

            // Compile ok
            if (!results.Errors.HasErrors)
            {
                LogAllTypes(results.CompiledAssembly);
                return l.ReturnAsOk(new AssemblyResult(assembly: results.CompiledAssembly, assemblyLocations: assemblyLocations));
            }

            // Compile error case
            var errors = "";
            foreach (CompilerError error in results.Errors)
            {
                var msg = $"Error ({error.ErrorNumber}): {error.ErrorText}";
                l.E(msg);
                errors += $"{msg}\n";
            }

            return l.ReturnAsError(new AssemblyResult(errorMessages: errors), errors);
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            var errorMessage = $"Error: Can't compile '{ThisAppCodeDll}' in {Path.GetFileName(relativePath)}. Details are logged into insights. {ex.Message}";
            return l.ReturnAsError(new AssemblyResult(errorMessages: errorMessage));
        }
    }

    private string[] GetAssemblyLocations(int appId)
    {
        var l = Log.Fn<string[]>($"{nameof(appId)}: {appId}");
        var tempAssemblyFolderPath = Path.Combine(_hostingEnvironment.MapPath("~/App_Data"), "2sxc.bin");
        l.A($"TempAssemblyFolderPath: '{tempAssemblyFolderPath}'");
        // Ensure "2sxc.bin" folder exists to preserve dlls
        Directory.CreateDirectory(tempAssemblyFolderPath);

        // need random name, because assemblies has to be preserved on disk, and we can not replace them until AppDomain is unloaded 
        var assemblyName = GetAppCodeDllName(tempAssemblyFolderPath, appId);
        l.A($"AssemblyName: '{assemblyName}'");
        var assemblyFilePath = Path.Combine(tempAssemblyFolderPath, $"{assemblyName}.dll");
        l.A($"AssemblyFilePath: '{assemblyFilePath}'");
        var symbolsFilePath = Path.Combine(tempAssemblyFolderPath, $"{assemblyName}.pdb");
        l.A($"SymbolsFilePath: '{symbolsFilePath}'");
        var assemblyLocations = new[] { symbolsFilePath, assemblyFilePath };
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