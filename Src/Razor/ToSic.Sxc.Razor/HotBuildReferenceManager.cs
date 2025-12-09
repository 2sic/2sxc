using Microsoft.CodeAnalysis;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Razor.DotNetOverrides;
using ToSic.Sxc.Sys;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Razor;

internal class HotBuildReferenceManager(
    RazorReferenceManager referenceManager,
    LazySvc<DependenciesLoader> dependenciesLoader,
    AssemblyResolver assemblyResolver,
    LazySvc<ExtensionCompileReferenceService> extensionReference)
    : ServiceBase($"{SxcLogging.SxcLogName}.HbRefMgr",
        connect: [referenceManager, dependenciesLoader, assemblyResolver, extensionReference])
{
    private readonly RazorReferenceManagerEnhanced _referenceManager = (RazorReferenceManagerEnhanced)referenceManager;

    internal IEnumerable<MetadataReference> GetMetadataReferences(string? appCodeFullPath, HotBuildSpec spec, string? sourcePath)
    {
        var additionalReferencePaths = new List<string>();
        try
        {
            if (spec != null! /* paranoid */ )
            {
                // TODO: need to invalidate this cache (_referencedAssembliesCache, _assemblyResolver, ...) if there is change in Dependencies folder
                var (dependencies, _) = dependenciesLoader.Value.TryGetOrFallback(spec);

                if (dependencies != null)
                {
                    assemblyResolver.AddAssemblies(dependencies);
                    additionalReferencePaths.AddRange(dependencies.Select(dependency => dependency.Location));
                }
            }

            if (!string.IsNullOrEmpty(appCodeFullPath) && File.Exists(appCodeFullPath))
                additionalReferencePaths.Add(appCodeFullPath);

            if (sourcePath.HasValue())
            {
                var referenceReader = extensionReference.Value;
                foreach (var reference in referenceReader.GetReferences(sourcePath, netFramework: false))
                {
                    var resolved = ExtensionCompileReferenceService.IsAssemblyName(reference.Value)
                        ? referenceReader.TryResolveAssemblyLocation(reference.Value)
                        : referenceReader.ResolveReferencePath(reference);

                    if (resolved.IsEmpty())
                    {
                        Log.W($"Extension reference '{reference.Value}' in '{reference.ExtensionFolder}' could not be resolved.");
                        continue;
                    }

                    if (!File.Exists(resolved))
                    {
                        Log.W($"Resolved reference '{resolved}' for '{reference.Value}' not found.");
                        continue;
                    }

                    additionalReferencePaths.Add(resolved);
                }
            }
        }
        catch
        {
            // ReSharper disable once EmptyStatement
        };

        return _referenceManager.GetAdditionalCompilationReferences(additionalReferencePaths);
    }
}
