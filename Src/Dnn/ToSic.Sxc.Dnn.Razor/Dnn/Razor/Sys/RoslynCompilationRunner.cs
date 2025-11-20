using System.Reflection;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Code.Sys.SourceCode;
using ToSic.Sxc.Dnn.Compile;

namespace ToSic.Sxc.Dnn.Razor.Sys;

/// <summary>
/// Handles the heavy lifting of compiling Razor templates and managing cache fallbacks.
/// Keeps <see cref="RoslynBuildManager"/> small and focused on orchestration.
/// </summary>
public class RoslynCompilationRunner(
    TemplateCacheService cacheService,
    RazorCompilerService razorCompiler,
    CSharpCompilerService csharpCompiler,
    IAssemblyDiskCacheService diskCacheService,
    IReferencedAssembliesProvider referencedAssembliesProvider,
    AssemblyUtilities assemblyUtilities,
    AssemblyResolver assemblyResolver,
    LazySvc<AppCodeLoader> appCodeLoader,
    RoslynCacheFallbackHandler fallbackHandler)
    : ServiceBase("Dnn.RzCmpRun", connect: [cacheService, razorCompiler, csharpCompiler, diskCacheService, referencedAssembliesProvider, assemblyUtilities, assemblyResolver, appCodeLoader, fallbackHandler])
{
    public AssemblyResult Compile(CodeFileInfo codeFileInfo, string className, HotBuildSpec spec)
    {
        var l = Log.Fn<AssemblyResult>($"{codeFileInfo}; {spec};", timer: true);

        var appCodeInfo = cacheService.GetAppCodeCacheInfo(spec);
        var (referencedAssemblies, appCodeAssemblyResult) = GetReferencedAssemblies(codeFileInfo, spec, appCodeInfo.AssemblyResult);

        var contentHash = diskCacheService.ComputeContentHash(codeFileInfo.SourceCode);
        var outputAssemblyPath = GetOutputAssemblyPath(codeFileInfo, spec, contentHash, appCodeInfo.Hash);
        var pathLowerCase = codeFileInfo.RelativePath!.ToLowerInvariant();
        var isCshtml = pathLowerCase.EndsWith(SourceCodeConstants.CsHtmlFileExtension, StringComparison.OrdinalIgnoreCase);

        if (isCshtml)
            className = assemblyUtilities.GetSafeClassName(codeFileInfo.FullPath);

        l.A($"Compiling template. Class: {className}; IsCshtml: {isCshtml}; Output: {outputAssemblyPath}");

        var (generatedAssembly, errors) = isCshtml
            ? razorCompiler.Compile(codeFileInfo.SourceCode, referencedAssemblies, className, codeFileInfo.FullPath, outputAssemblyPath)
            : csharpCompiler.Compile(codeFileInfo.SourceCode, referencedAssemblies, outputAssemblyPath);

        if (generatedAssembly == null)
        {
            var fallback = fallbackHandler.TryUseExisting(
                codeFileInfo,
                spec,
                className,
                isCshtml,
                contentHash,
                appCodeInfo,
                outputAssemblyPath,
                errors,
                appCodeAssemblyResult,
                assembly => CreateAssemblyResult(assembly, className, isCshtml, codeFileInfo, appCodeAssemblyResult));

            if (fallback != null)
                return l.ReturnAsOk(fallback);

            throw l.Ex(new Exception(
                $"Found {errors.Count} errors compiling '{codeFileInfo.FullPath}' " +
                $"(length: {codeFileInfo.SourceCode!.Length}, lines: {codeFileInfo.SourceCode.Split('\n').Length}): " +
                $"{assemblyUtilities.FormatCompilerErrors(errors)}")
            );
        }

        var assemblyResult = CreateAssemblyResult(generatedAssembly, className, isCshtml, codeFileInfo, appCodeAssemblyResult);
        cacheService.AddToCache(codeFileInfo, spec, assemblyResult, contentHash, appCodeInfo.Hash, appCodeAssemblyResult);
        return l.ReturnAsOk(assemblyResult);
    }

    private string GetOutputAssemblyPath(CodeFileInfo codeFileInfo, HotBuildSpec spec, string contentHash, string appCodeHash)
    {
        var l = Log.Fn<string>(timer: true);

        if (!diskCacheService.IsEnabled())
            return l.ReturnNull();

        var outputPath = diskCacheService.GetCacheFilePath(spec, codeFileInfo.RelativePath, contentHash, appCodeHash);
        if (!outputPath.HasValue())
            return l.ReturnNull($"{nameof(outputPath)} is null/empty");

        var directoryName = Path.GetDirectoryName(outputPath);
        if (directoryName.HasValue())
            Directory.CreateDirectory(directoryName!);

        return l.ReturnAsOk(outputPath);
    }

    private (List<string>, AssemblyResult) GetReferencedAssemblies(CodeFileInfo codeFileInfo, HotBuildSpec spec, AssemblyResult preloadedAppCode)
    {
        var l = Log.Fn<(List<string>, AssemblyResult)>(timer: true);

        var referencedAssemblies = referencedAssembliesProvider.Locations(codeFileInfo.RelativePath, spec) ?? [];
        var appCodeAssemblyResult = EnsureAppCodeAssembly(preloadedAppCode, spec);

        assemblyResolver.AddAssembly(appCodeAssemblyResult?.Assembly);

        var appCodeAssembly = appCodeAssemblyResult?.Assembly;
        if (appCodeAssembly?.Location is { } assemblyLocation && !referencedAssemblies.Contains(assemblyLocation, StringComparer.OrdinalIgnoreCase))
        {
            referencedAssemblies.Add(assemblyLocation);
            l.A($"Added reference to AppCode assembly: {assemblyLocation}");
        }

        return l.ReturnAsOk((referencedAssemblies, appCodeAssemblyResult));
    }

    private AssemblyResult EnsureAppCodeAssembly(AssemblyResult appCodeAssemblyResult, HotBuildSpec spec)
    {
        var l = Log.Fn<AssemblyResult>(timer: true);

        if (appCodeAssemblyResult != null)
            return l.Return(appCodeAssemblyResult, $"OK, {nameof(appCodeAssemblyResult)} has value.");

        var (loaded, _) = appCodeLoader.Value.GetAppCode(spec);
        return l.Return(loaded, $"{nameof(loaded)}");
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
}
