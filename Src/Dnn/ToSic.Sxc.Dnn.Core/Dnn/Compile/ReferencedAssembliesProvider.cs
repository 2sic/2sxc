using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Configuration;
using static System.StringComparer;

namespace ToSic.Sxc.Dnn.Compile
{
    [PrivateApi]
    public class ReferencedAssembliesProvider : IReferencedAssembliesProvider
    {
        
        private static readonly ConcurrentDictionary<string, List<string>> ReferencedAssembliesCache = new(InvariantCultureIgnoreCase);

        public List<string> Locations(string virtualPath)
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
