using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using System.CodeDom.Compiler;
using System.Reflection;
using ToSic.Sxc.Dnn.Compile;
using ToSic.Sys.Caching;

namespace ToSic.Sxc.Dnn.Razor.Sys;

/// <summary>
/// Service responsible for compiling C# code files into assemblies.
/// </summary>
public class CSharpCompilerService(
    MemoryCacheService memoryCacheService,
    IAssemblyDiskCacheService diskCacheService)
    : ServiceBase("Dnn.CsCmpSvc", connect: [memoryCacheService, diskCacheService])
{
    private const string CSharpCodeProviderCacheKey = "Sxc-Dnn-CSharpCodeProvider";
    private const int CSharpCodeProviderCacheMinutes = 5;

    /// <summary>
    /// Compiles C# source code into an assembly.
    /// </summary>
    public (Assembly Assembly, List<CompilerError> Errors) Compile(
        string sourceCode, 
        List<string> referencedAssemblies, 
        string outputAssemblyPath = null)
    {
        var l = Log.Fn<(Assembly, List<CompilerError>)>(timer: true, parameters: $"sourceCode: {sourceCode.Length} chars");

        var lTimer = Log.Fn("Compiler Params", timer: true);
        var compilerParameters = CreateCompilerParameters(referencedAssemblies, outputAssemblyPath);
        lTimer.Done();

        var compiler = GetCSharpCodeProvider();
        lTimer = Log.Fn("Compile", timer: true);
        var compilerResults = compiler.CompileAssemblyFromSource(compilerParameters, sourceCode);
        lTimer.Done();

        if (compilerResults.Errors.Count <= 0)
            return l.ReturnAsOk((compilerResults.CompiledAssembly, null));

        var errorList = compilerResults.Errors.Cast<CompilerError>().Where(e => !e.IsWarning).ToList();

        return (!errorList.Any())
            ? l.ReturnAsOk((compilerResults.CompiledAssembly, null))
            : l.ReturnAsError((null, errorList), "error");
    }

    private CSharpCodeProvider GetCSharpCodeProvider()
    {
        var l = Log.Fn<CSharpCodeProvider>(timer: true);

        if (memoryCacheService.TryGet<CSharpCodeProvider>(CSharpCodeProviderCacheKey, out var fromCache))
            return l.Return(fromCache, "from cached");

        var codeProvider = new CSharpCodeProvider();
        memoryCacheService.Set(CSharpCodeProviderCacheKey, codeProvider, p => p.SetSlidingExpiration(CSharpCodeProviderCacheMinutes * 60));

        return l.Return(codeProvider, "created new and cached");
    }

    private CompilerParameters CreateCompilerParameters(List<string> referencedAssemblies, string outputAssemblyPath)
    {
        var compilerParameters = new CompilerParameters([.. referencedAssemblies])
        {
            GenerateInMemory = !diskCacheService.IsEnabled() && string.IsNullOrEmpty(outputAssemblyPath),
            IncludeDebugInformation = true,
            TreatWarningsAsErrors = false,
            CompilerOptions = DnnRoslynConstants.CompilerOptions,
        };

        if (diskCacheService.IsEnabled() && outputAssemblyPath.HasValue())
        {
            var outDir = Path.GetDirectoryName(outputAssemblyPath);
            if (outDir.HasValue())
                Directory.CreateDirectory(outDir);

            compilerParameters.OutputAssembly = outputAssemblyPath;
            compilerParameters.TempFiles = new TempFileCollection(EnsureTempDir());
        }

        return compilerParameters;
    }

    private string EnsureTempDir()
    {
        var cacheRoot = diskCacheService.GetCacheDirectoryPath();
        var tempDir = Path.Combine(cacheRoot, "temp");
        if (!Directory.Exists(tempDir))
            Directory.CreateDirectory(tempDir);
        return tempDir;
    }
}
