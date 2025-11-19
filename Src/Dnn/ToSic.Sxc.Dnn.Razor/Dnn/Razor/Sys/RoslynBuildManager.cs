using System.Reflection;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Code.Sys.SourceCode;
using ToSic.Sxc.Dnn.Compile;
using ToSic.Sys.Locking;

namespace ToSic.Sxc.Dnn.Razor.Sys;

/// <summary>
/// This class is responsible for managing the compilation of Razor templates using Roslyn.
/// Orchestrates cache services and compiler services to build and retrieve compiled assemblies.
/// </summary>
public class RoslynBuildManager(
    LazySvc<AppCodeLoader> appCodeLoader,
    AssemblyResolver assemblyResolver,
    IReferencedAssembliesProvider referencedAssembliesProvider,
    IAssemblyDiskCacheService diskCacheService,
    TemplateCacheService cacheService,
    RazorCompilerService razorCompiler,
    CSharpCompilerService csharpCompiler,
    AssemblyUtilities assemblyUtilities)
    : ServiceBase("Dnn.RoslynBuildManager", connect: [appCodeLoader, assemblyResolver, referencedAssembliesProvider, diskCacheService, cacheService, razorCompiler, csharpCompiler, assemblyUtilities]),
        IRoslynBuildManager
{
    private static readonly NamedLocks CompileAssemblyLocks = new();

    /// <summary>
    /// Manage template compilations, cache the assembly and returns the generated type.
    /// </summary>
    public Type GetCompiledType(CodeFileInfo codeFileInfo, HotBuildSpec spec)
        => GetCompiledAssembly(codeFileInfo, null, spec)?.MainType;

    public AssemblyResult GetCompiledAssembly(CodeFileInfo codeFileInfo, string className, HotBuildSpec spec)
    {
        var l = Log.Fn<AssemblyResult>($"{codeFileInfo}; {spec};");

        var lockObject = CompileAssemblyLocks.Get(codeFileInfo.FullPath!);

        var (result, generated, message) = new TryLockTryDo(lockObject).Call(
            conditionToGenerate: () => ShouldCompile(codeFileInfo, spec),
            generator: () => CompileCodeFile(codeFileInfo, className, spec),
            cacheOrFallback: () => GetFromCacheFallback(codeFileInfo, spec)
        );

        if (!generated)
            LogCacheFallback(result, message, codeFileInfo, l);

        return l.Return(result, message);
    }

    private bool ShouldCompile(CodeFileInfo codeFileInfo, HotBuildSpec spec)
    {
        var l = Log.Fn<bool>();
        
        var cached = cacheService.TryGetFromCache(codeFileInfo, spec);
        
        if (cached?.MainType != null)
            return l.Return(false, "found in cache");

        l.A("Cache miss - will compile template");
        return l.Return(true, "not in cache");
    }

    private AssemblyResult GetFromCacheFallback(CodeFileInfo codeFileInfo, HotBuildSpec spec)
        => cacheService.TryGetFromCache(codeFileInfo, spec);

    private static void LogCacheFallback(AssemblyResult result, string message, CodeFileInfo codeFileInfo, ILog l)
    {
        l.A("Object retrieved from cache during lock wait");
        l.A($"{nameof(result)}: {result}");
        l.A($"{nameof(result.MainType)}: {result?.MainType}");
        l.A($"{nameof(result.HasAssembly)}: {result?.HasAssembly}");
        l.A($"{nameof(message)}: {message}");
        l.A($"{nameof(codeFileInfo)}: {codeFileInfo}");
    }

    private AssemblyResult CompileCodeFile(CodeFileInfo codeFileInfo, string className, HotBuildSpec spec)
    {
        var l = Log.Fn<AssemblyResult>($"{codeFileInfo}; {spec};", timer: true);

        var appCodeInfo = cacheService.GetAppCodeCacheInfo(spec);
        var (referencedAssemblies, appCodeAssemblyResult) = GetReferencedAssemblies(codeFileInfo, spec, appCodeInfo.AssemblyResult);

        var contentHash = diskCacheService.ComputeContentHash(codeFileInfo.SourceCode);
        var outputAssemblyPath = GetOutputAssemblyPath(codeFileInfo, spec, contentHash, appCodeInfo.Hash);
        var pathLowerCase = codeFileInfo.RelativePath!.ToLowerInvariant();
        var isCshtml = pathLowerCase.EndsWith(SourceCodeConstants.CsHtmlFileExtension);
        
        if (isCshtml) 
            className = assemblyUtilities.GetSafeClassName(codeFileInfo.FullPath);

        l.A($"Compiling template. Class: {className}; IsCshtml: {isCshtml}; Output: {outputAssemblyPath}");

        // Compile using appropriate compiler
        var (generatedAssembly, errors) = isCshtml
            ? razorCompiler.Compile(codeFileInfo.SourceCode, referencedAssemblies, className, codeFileInfo.FullPath, outputAssemblyPath)
            : csharpCompiler.Compile(codeFileInfo.SourceCode, referencedAssemblies, outputAssemblyPath);

        if (generatedAssembly == null)
            throw l.Ex(new Exception(
                $"Found {errors.Count} errors compiling '{codeFileInfo.FullPath}' " +
                $"(length: {codeFileInfo.SourceCode!.Length}, lines: {codeFileInfo.SourceCode.Split('\n').Length}): " +
                $"{assemblyUtilities.FormatCompilerErrors(errors)}")
            );

        // Create assembly result and cache it
        var assemblyResult = CreateAssemblyResult(generatedAssembly, className, isCshtml, codeFileInfo, appCodeAssemblyResult);
        
        cacheService.AddToCache(codeFileInfo, spec, assemblyResult, contentHash, appCodeInfo.Hash, appCodeAssemblyResult);

        return l.ReturnAsOk(assemblyResult);
    }

    private string GetOutputAssemblyPath(CodeFileInfo codeFileInfo, HotBuildSpec spec, string contentHash, string appCodeHash)
    {
        if (!diskCacheService.IsEnabled())
            return null;

        var normalizedPath = CacheKey.NormalizePath(codeFileInfo.RelativePath);
        var cacheDir = diskCacheService.GetCacheDirectoryPath();
        
        var outputPath = new CacheKey(spec.AppId, spec.Edition, normalizedPath, contentHash, appCodeHash).GetFilePath(cacheDir);
        var directoryName = Path.GetDirectoryName(outputPath);
        if (directoryName.HasValue())
            Directory.CreateDirectory(directoryName!);
        
        return outputPath;
    }

    private AssemblyResult CreateAssemblyResult(Assembly generatedAssembly, string className, bool isCshtml, CodeFileInfo codeFileInfo, AssemblyResult appCodeAssemblyResult)
    {
        var l = Log.Fn<AssemblyResult>(timer: true);

        var mainType = assemblyUtilities.FindMainType(generatedAssembly, className, isCshtml);
        l.A($"Main type: {mainType}");

        var assemblyResult = new AssemblyResult(generatedAssembly)
        {
            SafeClassName = className,
            MainType = mainType,
            CacheDependencyId = AssemblyCacheManager.KeyTemplate(codeFileInfo.FullPath!),
            AppCodeDependency = appCodeAssemblyResult
        };

        return l.ReturnAsOk(assemblyResult);
    }

    private (List<string>, AssemblyResult) GetReferencedAssemblies(CodeFileInfo codeFileInfo, HotBuildSpec spec, AssemblyResult preloadedAppCode)
    {
        var l = Log.Fn<(List<string>, AssemblyResult)>(timer: true);

        var referencedAssemblies = referencedAssembliesProvider.Locations(codeFileInfo.RelativePath, spec);
        var appCodeAssemblyResult = EnsureAppCodeAssembly(preloadedAppCode, spec);

        assemblyResolver.AddAssembly(appCodeAssemblyResult?.Assembly);

        var appCodeAssembly = appCodeAssemblyResult?.Assembly;
        if (appCodeAssembly != null)
        {
            var assemblyLocation = appCodeAssembly.Location;
            referencedAssemblies.Add(assemblyLocation);
            l.A($"Added reference to AppCode assembly: {assemblyLocation}");
        }

        return l.ReturnAsOk((referencedAssemblies, appCodeAssemblyResult));
    }

    private AssemblyResult EnsureAppCodeAssembly(AssemblyResult appCodeAssemblyResult, HotBuildSpec spec)
    {
        if (appCodeAssemblyResult != null)
            return appCodeAssemblyResult;

        var (loaded, _) = appCodeLoader.Value.GetAppCode(spec);
        return loaded;
    }
}
