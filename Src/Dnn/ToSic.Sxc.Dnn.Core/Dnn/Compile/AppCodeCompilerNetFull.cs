using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using ToSic.Eav.Internal.Configuration;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Dnn.Compile;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Code;

[PrivateApi]
internal class AppCodeCompilerNetFull(IHostingEnvironmentWrapper hostingEnvironment, IReferencedAssembliesProvider referencedAssembliesProvider, IGlobalConfiguration globalConfiguration, SourceCodeHasher sourceCodeHasher)
    : AppCodeCompiler(globalConfiguration, sourceCodeHasher, connect: [hostingEnvironment, referencedAssembliesProvider, sourceCodeHasher])
{

    protected internal override AssemblyResult GetAppCode(string relativePath, HotBuildSpecWithSharedSuffix spec)
    {
        var l = Log.Fn<AssemblyResult>($"{nameof(relativePath)}: '{relativePath}'; {spec}");

        try
        {
            // store it, so we can provide it to _referencedAssembliesProvider.Locations()
            _relativePath = relativePath;
            _spec = spec;

            // Get all C# files in the folder
            var sourceRootPath = NormalizeFullPath(hostingEnvironment.MapPath(relativePath));
            var sourceFiles = GetSourceFiles(sourceRootPath); // stv# this iterator can calculate hash, so we can iterate file once
            if (sourceFiles.Length == 0)
                return l.ReturnAsOk(new());

            var (symbolsPath, assemblyPath) = GetAssemblyLocations(spec, sourceRootPath);

            var results = File.Exists(assemblyPath)
                ? new(new TempFileCollection()) { PathToAssembly = assemblyPath, CompiledAssembly = Assembly.LoadFrom(assemblyPath) }
                : GetCompiledAssemblyFromFolder(sourceFiles, assemblyPath);

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
                return l.ReturnAsOk(new(assembly: results.CompiledAssembly, assemblyLocations: [symbolsPath, assemblyPath], infos: dicInfos));
            }

            // Compile error case
            var errors = "";
            foreach (var msg in results.Errors.Cast<CompilerError>().Where(error => !error.IsWarning).ToList()
                .Select(error => $"Error ({error.ErrorNumber}): {error.ErrorText} in '{error.FileName}' (Line: {error.Line}, Column: {error.Column})."))
            {
                l.E(msg);
                errors += $"{msg}\n";
            }
            foreach (var msg in results.Errors.Cast<CompilerError>().Where(error => error.IsWarning).ToList()
                .Select(warning => $"Warning ({warning.ErrorNumber}): {warning.ErrorText} in '{warning.FileName}' (Line: {warning.Line}, Column: {warning.Column})."))
            {
                l.W(msg);
                errors += $"{msg}\n";
            }

            return l.ReturnAsError(new(errorMessages: errors, infos: dicInfos), errors);
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            var errorMessage = $"Error: Can't compile '{AppCodeDll}' in {Path.GetFileName(relativePath)}. Details are logged into insights. {ex.Message}";
            return l.ReturnAsError(new(errorMessages: errorMessage));
        }
    }
    private string _relativePath;
    private HotBuildSpec _spec;

    //// Loads the content of a file to a byte array.
    //private static byte[] LoadFile(string filename)
    //{
    //    using var fs = new FileStream(filename, FileMode.Open);
    //    {
    //        var buffer = new byte[(int)fs.Length];
    //        fs.Read(buffer, 0, buffer.Length);
    //        return buffer;
    //    }
    //}

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
        parameters.ReferencedAssemblies.AddRange(referencedAssembliesProvider.Locations(_relativePath, _spec).ToArray());

        var codeProvider = new CSharpCodeProvider();
        var compilerResults = codeProvider.CompileAssemblyFromFile(parameters, sourceFiles);

        return compilerResults.Errors.HasErrors
            ? l.ReturnAsError(compilerResults)
            : l.ReturnAsOk(compilerResults);
    }
}