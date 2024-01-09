﻿using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace ToSic.Sxc.Code.Internal.HotBuild;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AssemblyResolver
{

    private static bool _isHandlerRegistered = false;
    private readonly ConcurrentDictionary<string, Assembly> _assemblyCache = new();
    private readonly ConcurrentDictionary<string, string> _assemblyPathPerApp = new(StringComparer.InvariantCultureIgnoreCase);

    public AssemblyResolver()
    {
        if (!_isHandlerRegistered)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomain_AssemblyResolve;
            AppDomain.CurrentDomain.TypeResolve += CurrentDomain_AssemblyResolve;
            _isHandlerRegistered = true;
        }
    }

    private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        => _assemblyCache.TryGetValue(args.Name, out var assembly) ? assembly : null;

    public void AddAssembly(Assembly assembly, string appRelativePath = null)
    {
        if (assembly == null) return;
        _assemblyCache.TryAdd(assembly.GetName().FullName, assembly);

        if (appRelativePath != null) _assemblyPathPerApp.TryAdd(appRelativePath, assembly.Location);
    }

    public string GetAssemblyLocation(string appRelativePath)
        => _assemblyPathPerApp.TryGetValue(appRelativePath, out var location) ? location : null;
}