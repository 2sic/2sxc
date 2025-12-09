using System.Collections.Concurrent;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Configuration;
using System.Web.Hosting;
using ToSic.Sxc.Code.Sys.HotBuild;
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
            ReferenceAssembly(referencedAssemblies, assembly.Assembly);
        lTimer.Done();

        // include assemblies from `\AppCode\Extensions\[extension-name]\compile.json.resources`
        lTimer = Log.Fn("timer for Extensions Reference Assemblies", timer: true);
        EnsureExtensionsReferenceAssemblies(referencedAssemblies, virtualPath);
        lTimer.Done();

        lTimer = Log.Fn("timer for Dependencies", timer: true);
        if (spec != null)
        {
            // TODO: need to invalidate this cache (_referencedAssembliesCache, _assemblyResolver, ...) if there is change in Dependencies folder
            var (dependencies, _) = dependenciesLoader.TryGetOrFallback(spec);
            assemblyResolver.AddAssemblies(dependencies);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
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

    private static void ReferenceAssembly(ICollection<string> referencedAssemblies, string assemblyName)
    {
        if (assemblyName.IsEmpty())
            return;

        var normalized = ExtensionCompileReferenceReader.NormalizeAssemblyName(assemblyName);
        if (HasAssembly(referencedAssemblies, $"{normalized}.dll"))
            return;

        // Process your assembly information here
        try
        {
            referencedAssemblies.Add(Assembly.ReflectionOnlyLoad(normalized).Location);
        }
        catch
        {
            // sink
        }
    }


    private void EnsureExtensionsReferenceAssemblies(ICollection<string> referencedAssemblies, string virtualPath)
    {
        //foreach (var assemblyName in GetExtensionsReferenceAssemblyNames)
        //    referencedAssemblies.Add(Assembly.ReflectionOnlyLoad(assemblyName).Location);

        var physicalPath = MapVirtualPath(virtualPath);
        if (physicalPath.IsEmpty())
            return;

        foreach (var reference in ExtensionCompileReferenceReader.GetReferences(physicalPath, netFramework: true))
        {
            if (ExtensionCompileReferenceReader.IsAssemblyName(reference.Value))
            {
                ReferenceAssembly(referencedAssemblies, reference.Value);
                continue;
            }

            var resolvedPath = ExtensionCompileReferenceReader.ResolveReferencePath(reference);
            if (resolvedPath.IsEmpty() || !File.Exists(resolvedPath))
            {
                Log.W($"Extension reference '{reference.Value}' in '{reference.ExtensionFolder}' not found or unreadable.");
                continue;
            }

            referencedAssemblies.Add(resolvedPath);
        }
    }

    private static string MapVirtualPath(string virtualPath)
    {
        if (virtualPath.IsEmpty())
            return string.Empty;

        try
        {
            return HostingEnvironment.MapPath(virtualPath);
        }
        catch
        {
            return string.Empty;
        }
    }

    private static bool HasAssembly(IEnumerable<string> referencedAssemblies, string fileName)
        => referencedAssemblies.Any(path => string.Equals(Path.GetFileName(path), fileName, StringComparison.InvariantCultureIgnoreCase));

    //private static readonly string[] GetExtensionsReferenceAssemblyNames =
    //{
    //    "System.Net.Http, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    //};

    ///// <summary>
    ///// We need to skip invalid assemblies to not break compile process
    ///// </summary>
    ///// <param name="filePath"></param>
    ///// <returns></returns>
    //private static bool IsValidAssembly(string filePath)
    //{
    //    try
    //    {
    //        Assembly.ReflectionOnlyLoadFrom(filePath);
    //        return true;
    //    }
    //    catch
    //    {
    //        return false;
    //    }
    //}

    // static cached, because in case of dll change app will restart itself
    private static IReadOnlyList<string> AppReferencedAssemblies()
        => _appReferenceAssemblies ??= BuildManager.GetReferencedAssemblies().Cast<Assembly>().Select(assembly => assembly/*.WithPolicy()*/.Location).ToList().AsReadOnly();

    private static IReadOnlyList<string> _appReferenceAssemblies;
}
