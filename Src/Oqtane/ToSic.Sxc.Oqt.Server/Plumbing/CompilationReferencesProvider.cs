using System.Collections;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

// Used by RazorView compilation.
// Based on https://stackoverflow.com/questions/58685966/adding-assemblies-types-to-be-made-available-to-razor-page-at-runtime to work
namespace ToSic.Sxc.Oqt.Server.Plumbing;

internal class CompilationReferencesProvider(Assembly assembly) : AssemblyPart(assembly), ICompilationReferencesProvider
{
    private readonly Assembly _assembly = assembly;

    public IEnumerable<string> GetReferencePaths()
    {
        // your `LoadPrivateBinAssemblies()` method needs to be called before the next line executes!
        // So you should load all private bin's before the first RazorPage gets requested....

        // 1. Ensure we don't run into null problems
        var loadContext = AssemblyLoadContext.GetLoadContext(_assembly);
        if (loadContext == null) return Enumerable.Empty<string>();

        var nonDynamicAssemblies = loadContext.Assemblies
            .Where(_ => !_.IsDynamic)
            .ToList();

        // 2. use new code with location. has some duplicates and many "" empty strings
        // which seem to be merged dynamic libraries - usually the CodeBase called them "System.Private.CoreLib.dll"
        // But that one is already included
        var newer = nonDynamicAssemblies.Select(_ => _.Location).ToList();
        var newerDistinct = newer.Distinct().Where(path => !path.IsNullOrEmpty()).ToList();

        return newerDistinct;
    }
}