using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Configuration;
using ToSic.Lib.Services;
using ToSic.Sxc.Code.Internal.HotBuild;
using static System.StringComparer;

namespace ToSic.Sxc.Dnn.Compile;

[PrivateApi]
public class ReferencedAssembliesProvider(DependenciesLoader dependenciesLoader, AssemblyResolver assemblyResolver)
    : ServiceBase("Sxc.RefAP", connect: [dependenciesLoader, assemblyResolver]), IReferencedAssembliesProvider
{
    // cache of referenced assemblies per virtual path
    private static readonly ConcurrentDictionary<string, List<string>> ReferencedAssembliesCache = new(InvariantCultureIgnoreCase);

    public List<string> Locations(string virtualPath, HotBuildSpec spec)
    {
        var l = Log.Fn<List<string>>($"for: '{virtualPath}'");
        if (ReferencedAssembliesCache.TryGetValue(virtualPath, out var cachedResult))
            return l.Return(new(cachedResult), "cached, re-wrapped in new list");

        var lTimer = Log.Fn("timer for AppRef", timer: true);
        var referencedAssemblies = new List<string>(AppReferencedAssemblies());
        lTimer.Done();

        // include assemblies from compilation section in web.config hierarchy
        lTimer = Log.Fn("timer for Web Configuration Manager", timer: true);
        var compilationSection = (CompilationSection)WebConfigurationManager.GetSection("system.web/compilation", virtualPath);
        foreach (AssemblyInfo assembly in compilationSection.Assemblies)
        {
            // Process your assembly information here
            try
            {
                //referencedAssemblies.Add(assembly.WithPolicy().Location);
                referencedAssemblies.Add(Assembly.ReflectionOnlyLoad(assembly.Assembly).Location);
            }
            catch
            {
                // sink
            }
        }
        lTimer.Done();

        lTimer = Log.Fn("timer for Dependencies", timer: true);
        if (spec != null)
        {
            // TODO: need to invalidate this cache (_referencedAssembliesCache, _assemblyResolver, ...) if there is change in Dependencies folder
            var (dependencies, _) = dependenciesLoader.TryGetOrFallback(spec);
            assemblyResolver.AddAssemblies(dependencies);

            if (dependencies != null)
                foreach (var dependency in dependencies)
                    referencedAssemblies.Add(dependency.Location);
        }
        lTimer.Done();

        // deduplicate referencedAssemblies by filename, keep last duplicate
        referencedAssemblies = referencedAssemblies
            //.Where(IsValidAssembly)
            .GroupBy(Path.GetFileName)
            .Select(g => g.Last())
            .ToList();

        ReferencedAssembliesCache.TryAdd(virtualPath, referencedAssemblies);

        return l.Return(new(referencedAssemblies), "created, re-wrapped in new list");
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
    private static IReadOnlyList<string> AppReferencedAssemblies()
        => _appReferenceAssemblies ??= BuildManager.GetReferencedAssemblies().Cast<Assembly>().Select(assembly => assembly/*.WithPolicy()*/.Location).ToList().AsReadOnly();

    private static IReadOnlyList<string> _appReferenceAssemblies;
}
