using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToSic.Lib.Services;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Internal;
using ToSic.Sxc.Razor.DbgWip;

namespace ToSic.Sxc.Razor
{
    internal class HotBuildReferenceManager : ServiceBase
    {
        private readonly RazorReferenceManagerEnhanced _referenceManager;
        private readonly Lazy<DependenciesLoader> _dependenciesLoader;
        private readonly AssemblyResolver _assemblyResolver;

        public HotBuildReferenceManager(RazorReferenceManager referenceManager, Lazy<DependenciesLoader> dependenciesLoader, AssemblyResolver assemblyResolver) : base($"{SxcLogging.SxcLogName}.HbRefMgr")
        {
            ConnectServices(
                _referenceManager = (RazorReferenceManagerEnhanced)referenceManager,
                _dependenciesLoader = dependenciesLoader,
                _assemblyResolver = assemblyResolver
            );
        }

        internal IEnumerable<MetadataReference> GetMetadataReferences(string appCodeFullPath, HotBuildSpec spec)
        {
            var additionalReferencePaths = new List<string>();
            try
            {
                if (spec != null)
                {
                    // TODO: need to invalidate this cache (_referencedAssembliesCache, _assemblyResolver, ...) if there is change in Dependencies folder
                    var (dependencies, _) = _dependenciesLoader.Value.TryGetOrFallback(spec);
                    _assemblyResolver.AddAssemblies(dependencies);

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
