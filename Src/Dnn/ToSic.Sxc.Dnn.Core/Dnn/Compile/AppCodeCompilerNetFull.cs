using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Code.Sys.SourceCode;
using ToSic.Sxc.Dnn.Compile;
using ToSic.Sys.Configuration;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Code;

[PrivateApi]
internal class AppCodeCompilerNetFull(
    IHostingEnvironmentWrapper hostingEnvironment,
    IReferencedAssembliesProvider referencedAssembliesProvider,
    IGlobalConfiguration globalConfiguration,
    SourceCodeHasher sourceCodeHasher)
    : AppCodeCompiler(globalConfiguration, sourceCodeHasher, connect: [hostingEnvironment, referencedAssembliesProvider, sourceCodeHasher])
{
    public override AssemblyResult GetAppCode(string relativePath, HotBuildSpecWithSharedSuffix spec)
    {
        var l = Log.Fn<AssemblyResult>($"{nameof(relativePath)}: '{relativePath}'; {spec}", timer: true);

        try
        {
            // Resolve source files
            var sourceRootPath = NormalizeFullPath(hostingEnvironment.MapPath(relativePath));
            var sourceFiles = GetSourceFiles(sourceRootPath);
            if (sourceFiles.Length == 0)
                return l.ReturnAsOk(new());

            // Target locations
            var (symbolsPath, assemblyPath) = GetAssemblyLocations(spec, sourceRootPath);
            var dllName = Path.GetFileName(assemblyPath);

            // Build or reuse compiled assembly
            var result = LockAppCodeAssemblyProvider.Call(
                conditionToGenerate: () => ShouldGenerate(assemblyPath),
                generator: () => GetCompiledAssemblyFromFolder(sourceFiles, assemblyPath, relativePath, spec),
                cacheOrFallback: () => new(new TempFileCollection())
                {
                    PathToAssembly = assemblyPath,
                    CompiledAssembly = Assembly.LoadFrom(assemblyPath)
                }
            );

            var compilerResults = result.Result;

            var dicInfos = new Dictionary<string, string>
            {
                ["DllName"] = dllName,
                ["Files"] = sourceFiles.Length.ToString(),
                ["Errors"] = compilerResults.Errors.HasErrors.ToString(),
                ["Assembly"] = compilerResults.CompiledAssembly?.FullName ?? "null",
                ["AssemblyPath"] = assemblyPath,
                ["SymbolsPath"] = symbolsPath,
            };

            // Success
            if (!compilerResults.Errors.HasErrors)
            {
                LogAllTypes(compilerResults.CompiledAssembly);
                return l.ReturnAsOk(new(assembly: compilerResults.CompiledAssembly)
                {
                    AssemblyLocations = [symbolsPath, assemblyPath],
                    Infos = dicInfos,
                });
            }

            // Errors and warnings
            var errorMessages = GetErrorMessages(compilerResults, l);
            return l.ReturnAsError(new()
            {
                ErrorMessages = errorMessages,
                Infos = dicInfos,
            }, errorMessages);
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            var errorMessage = $"Error: Can't compile '{AppCodeDll}' in {Path.GetFileName(relativePath)}. Details are logged into insights. {ex.Message}";
            return l.ReturnAsError(new() { ErrorMessages = errorMessage, });
        }
    }

    private static string GetErrorMessages(CompilerResults compilerResults, ILogCall<AssemblyResult> l)
    {
        // first return all errors and then all warnings
        var errorsSb = new StringBuilder();
        var warningsSb = new StringBuilder();
        foreach (CompilerError ce in compilerResults.Errors)
        {
            var msg = $"{(ce.IsWarning ? "Warning" : "Error")} ({ce.ErrorNumber}): {ce.ErrorText} in '{ce.FileName}' (Line: {ce.Line}, Column: {ce.Column}).";
            if (ce.IsWarning)
            {
                l.W(msg);
                warningsSb.AppendLine(msg);
            } else
            { 
                l.E(msg);
                errorsSb.AppendLine(msg);
            }
        }
        var errors = errorsSb.Append(warningsSb).ToString();
        return errors;
    }

    private CompilerResults GetCompiledAssemblyFromFolder(string[] sourceFiles, string assemblyFilePath, string relativePath, HotBuildSpec spec)
    {
        var l = Log.Fn<CompilerResults>($"{nameof(sourceFiles)}: {sourceFiles.Length}; {nameof(assemblyFilePath)}: '{assemblyFilePath}'", timer: true);

        // Save to disk so it can be loaded by runtime
        var parameters = new CompilerParameters(null, assemblyFilePath)
        {
            GenerateInMemory = false,
            GenerateExecutable = false,
            IncludeDebugInformation = true,
            CompilerOptions = DnnRoslynConstants.CompilerOptions,
        };

        // Referenced assemblies
        parameters.ReferencedAssemblies.AddRange(referencedAssembliesProvider.Locations(relativePath, spec).ToArray());

        using var codeProvider = new CSharpCodeProvider();
        var compilerResults = codeProvider.CompileAssemblyFromFile(parameters, sourceFiles);

        return compilerResults.Errors.HasErrors
            ? l.ReturnAsError(compilerResults)
            : l.ReturnAsOk(compilerResults);
    }
}