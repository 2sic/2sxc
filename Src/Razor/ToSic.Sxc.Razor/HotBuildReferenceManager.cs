using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToSic.Lib.Services;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Internal;
using ToSic.Sxc.Razor.DotNetOverrides;

namespace ToSic.Sxc.Razor
{
    internal class HotBuildReferenceManager(
        RazorReferenceManager referenceManager,
        Lazy<DependenciesLoader> dependenciesLoader,
        AssemblyResolver assemblyResolver)
        : ServiceBase($"{SxcLogging.SxcLogName}.HbRefMgr",
            connect: [referenceManager, dependenciesLoader, assemblyResolver])
    {
        private readonly RazorReferenceManagerEnhanced _referenceManager = (RazorReferenceManagerEnhanced)referenceManager;

        internal IEnumerable<MetadataReference> GetMetadataReferences(string appCodeFullPath, HotBuildSpec spec)
        {
            var additionalReferencePaths = new List<string>();
            try
            {
                if (spec != null)
                {
                    // TODO: need to invalidate this cache (_referencedAssembliesCache, _assemblyResolver, ...) if there is change in Dependencies folder
                    var (dependencies, _) = dependenciesLoader.Value.TryGetOrFallback(spec);
                    assemblyResolver.AddAssemblies(dependencies);

                    if (dependencies != null) additionalReferencePaths.AddRange(dependencies.Select(dependency => dependency.Location));
                }

                if (!string.IsNullOrEmpty(appCodeFullPath) && File.Exists(appCodeFullPath))
                    additionalReferencePaths.Add(appCodeFullPath);
            }
            catch
            {
                // sink
            };

            return _referenceManager.GetAdditionalCompilationReferences(additionalReferencePaths);
        }
    }
}
