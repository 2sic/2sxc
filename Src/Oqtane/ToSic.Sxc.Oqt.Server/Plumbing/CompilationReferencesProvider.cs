using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

//[assembly: ProvideApplicationPartFactory(typeof(MyApplicationPartFactory))]

// WIP trying to get https://stackoverflow.com/questions/58685966/adding-assemblies-types-to-be-made-available-to-razor-page-at-runtime to work

namespace ToSic.Sxc.Oqt.Server.Plumbing
{
    public class CompilationReferencesProvider: AssemblyPart, ICompilationReferencesProvider
    {
        private readonly Assembly _assembly;

        public CompilationReferencesProvider(Assembly assembly) : base(assembly)
        {
            _assembly = assembly;
        }

        public IEnumerable<string> GetReferencePaths()
        {
            // your `LoadPrivateBinAssemblies()` method needs to be called before the next line executes!
            // So you should load all private bin's before the first RazorPage gets requested.

            //return new List<string>();

            return AssemblyLoadContext.GetLoadContext(_assembly).Assemblies
                .Where(_ => !_.IsDynamic)
                .Select(_ => new Uri(_.Location).LocalPath);
        }
    }
}
