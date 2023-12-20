using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace ToSic.Sxc.Code
{
    public class AssemblyResolver
    {

        private static bool _isHandlerRegistered = false;
        private readonly ConcurrentDictionary<string, Assembly> _assemblyCache = new();

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

        public void AddAssembly(Assembly assembly)
        {
            if (assembly == null) return;
            _assemblyCache.TryAdd(assembly.GetName().FullName, assembly);
        }
    }
}
