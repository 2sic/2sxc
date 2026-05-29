using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;
using ToSic.Sxc.Razor.DotNetOverrides;

namespace ToSic.Sxc.Razor;

internal class RazorReferenceManagerEnhanced(ApplicationPartManager partManager, IOptions<MvcRazorRuntimeCompilationOptions> options) : RazorReferenceManager(partManager, options)
{
    private IReadOnlyList<string>? _compilationReferencePaths;
    private IReadOnlyList<MetadataReference>? _compilationReferences;

    // Cache the optimized base reference set.
    // The base ASP.NET Core RazorReferenceManager returns all references reported by ApplicationParts.
    // In Oqtane upgrade scenarios that list can contain stale root DLLs, duplicate root/refs assemblies,
    // and runtime implementation assemblies which Roslyn should not compile against.
    // So we keep the cache, but cache the cleaned path list instead of the raw base.CompilationReferences.
    public override IReadOnlyList<MetadataReference> CompilationReferences
        => _compilationReferences ??= CompilationReferencePaths
            .Select(CreateMetadataReference)
            .ToList()
            .AsReadOnly();

    private IReadOnlyList<string> CompilationReferencePaths
        => _compilationReferencePaths ??= RazorReferencePathOptimizer.PreferCompileReferences(GetReferencePaths());

    public IReadOnlyList<MetadataReference> GetAdditionalCompilationReferences(IEnumerable<string> additionalReferencePaths)
    {
        if (additionalReferencePaths == null! /* paranoid */)
            return CompilationReferences;

        // AppCode, extension references, and dependency DLLs are added per compile.
        // Re-run the same optimizer over base + additional paths so a view/app reference can not
        // accidentally reintroduce a duplicate or stale framework DLL which was filtered from the base set.
        var referencePaths = RazorReferencePathOptimizer
            .PreferCompileReferences(CompilationReferencePaths.Concat(additionalReferencePaths));

        return referencePaths
            .Select(CreateMetadataReference)
            .ToList()
            .AsReadOnly();
    }
}
