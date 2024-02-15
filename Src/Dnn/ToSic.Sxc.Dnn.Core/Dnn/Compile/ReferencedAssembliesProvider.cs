using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Configuration;
using ToSic.Lib.Services;
using ToSic.Sxc.Code.Internal.HotBuild;
using static System.StringComparer;

namespace ToSic.Sxc.Dnn.Compile
{
    [PrivateApi]
    public class ReferencedAssembliesProvider : ServiceBase, IReferencedAssembliesProvider
    {
        private readonly DependenciesLoader _dependenciesLoader;
        private readonly AssemblyResolver _assemblyResolver;

        // cache of referenced assemblies per virtual path
        private static readonly ConcurrentDictionary<string, List<string>> ReferencedAssembliesCache = new(InvariantCultureIgnoreCase);

        public ReferencedAssembliesProvider(DependenciesLoader dependenciesLoader, AssemblyResolver assemblyResolver) : base("Sxc.RefAP")
        {
            ConnectServices(
                _dependenciesLoader = dependenciesLoader,
                _assemblyResolver = assemblyResolver
                );
        }

        public List<string> Locations(string virtualPath, HotBuildSpec spec)
        {
            if (ReferencedAssembliesCache.TryGetValue(virtualPath, out var cachedResult))
                return [..cachedResult];

            var referencedAssemblies = new List<string>(AppReferencedAssemblies());

            // include assemblies from compilation section in web.config hierarchy
            var compilationSection = (CompilationSection)WebConfigurationManager.GetSection("system.web/compilation", virtualPath);
            foreach (AssemblyInfo assembly in compilationSection.Assemblies)
            {
                // Process your assembly information here
                try
                {
                    var assemblyName = assembly.Assembly;
                    var a = Assembly.ReflectionOnlyLoad(assemblyName);
                    referencedAssemblies.Add(a.Location);
                }
                catch
                {
                    // sink
                }
            }

            if (spec != null)
            {
                // TODO: need to invalidate this cache (_referencedAssembliesCache, _assemblyResolver, ...) if there is change in Dependencies folder
                var (dependencies, _) = _dependenciesLoader.TryGetOrFallback(spec);
                _assemblyResolver.AddAssemblies(dependencies);

                if (dependencies != null)
                    foreach (var dependency in dependencies)
                        referencedAssemblies.Add(dependency.Location);
            }

            // deduplicate referencedAssemblies by filename, keep last duplicate
            referencedAssemblies = referencedAssemblies
                //.Where(IsValidAssembly)
                .GroupBy(Path.GetFileName)
                .Select(g => g.Last())
                .ToList();

            ReferencedAssembliesCache.TryAdd(virtualPath, referencedAssemblies);

            return [..referencedAssemblies];
        }

        /// <summary>
        /// We need to skip invalid assemblies to not break compile process
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static bool IsValidAssembly(string filePath)
        {
            try
            {
                Assembly.ReflectionOnlyLoadFrom(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // static cached, because in case of dll change app will restart itself
        private static IEnumerable<string> AppReferencedAssemblies() =>
            (from Assembly assembly in BuildManager.GetReferencedAssemblies() select assembly.Location);
    }
}
