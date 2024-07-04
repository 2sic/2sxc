using System.Collections.Concurrent;
using System.Reflection;
using ToSic.Eav.Helpers;
using ToSic.Lib.Services;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Code.Internal.HotBuild;

/// <summary>
/// Special SINGLETON service to resolve assemblies.
/// The purpose is to ensure .net can access assemblies which are compiled at runtime.
/// </summary>
/// <remarks>
/// This is a singleton!
/// </remarks>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AssemblyResolver : ServiceBase, ILogShouldNeverConnect
{
    private static bool _isHandlerRegistered = false;
    private readonly ConcurrentDictionary<string, Assembly> _assemblyCache = new();
    private readonly ConcurrentDictionary<string, string> _assemblyPathPerApp = new(StringComparer.InvariantCultureIgnoreCase);

#if DEBUG
    private const bool Debug = true;
#else
    private const bool Debug = false;
#endif

    public AssemblyResolver(ILogStore logStore) : base($"{SxcLogging.SxcLogName}.AsmRsl")
    {
        var l = Debug ? Log.Fn() : null;
        
        if (_isHandlerRegistered)
        {
            l.Done("already registered");
            return;
        }

        if (Debug)
            logStore.Add("system-assemblyresolver", Log);

        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomain_AssemblyResolve;
        _isHandlerRegistered = true;
        l.Done("registered first time");
    }

    private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        var l = Debug ? Log.Fn<Assembly>($"{nameof(sender)}:'{sender}'; {nameof(args.Name)}:'{args.Name}'; {nameof(args.RequestingAssembly)}:'{args.RequestingAssembly}'") : null;
        var r = _assemblyCache.TryGetValue(args.Name, out var assembly) ? assembly : null;
        return l.Return(r, (r != null) ? "Ok" : "can't find");
    }

    public void AddAssemblies(List<Assembly> assemblies, string appRelativePath = null)
    {
        var l = Debug ? Log.Fn($"{nameof(assemblies)}:'{assemblies?.Count}'; {nameof(appRelativePath)}:'{appRelativePath}'") : null;
        if (assemblies == null)
        {
            l.Done("assemblies is null");
            return;
        }
        foreach (var assembly in assemblies) 
            AddAssembly(assembly, appRelativePath);
        l.Done("Ok");
    }

    public void AddAssembly(Assembly assembly, string appRelativePath = null)
    {
        var l = Debug ? Log.Fn($"{nameof(assembly)}:'{assembly}'; {nameof(appRelativePath)}:'{appRelativePath}'") : null;
        if (assembly == null)
        {
            l.Done("assembly is null");
            return;
        }

        l.A(_assemblyCache.TryAdd(assembly.GetName().FullName, assembly)
            ? $"added to cache: '{assembly.GetName().FullName}'"
            : $"already in cache: '{assembly.GetName().FullName}'");

        if (appRelativePath != null && !string.IsNullOrEmpty(assembly.Location))
        {
            _assemblyPathPerApp[appRelativePath] = assembly.Location;
            l.A($"add or update {nameof(_assemblyPathPerApp)} {nameof(appRelativePath)}:'{appRelativePath}'; {nameof(assembly.Location)}:'{assembly.Location}'");
        }
        l.Done("Ok");
    }

    public string GetAssemblyLocation(string appRelativePath)
    {
        appRelativePath = appRelativePath?.Backslash();
        var l = Debug ? Log.Fn<string>($"{nameof(appRelativePath)}:'{appRelativePath}'") : null; 
        var r = _assemblyPathPerApp.TryGetValue(appRelativePath, out var location) ? location : null;
        return l.Return(r, (r != null) ? $"Ok:'{r}'" : "can't find");
    }
}