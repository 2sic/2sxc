using System.Reflection;
using System.Runtime.Loader;

namespace ToSic.Sxc.Razor
{

    // Create a collectible AssemblyLoadContext
    // https://docs.microsoft.com/en-us/dotnet/standard/assembly/unloadability#create-a-collectible-assemblyloadcontext
    public class UnloadableAssemblyLoadContext : AssemblyLoadContext
    {
        public UnloadableAssemblyLoadContext() : base(isCollectible: true)
        {
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null; // That means that all the dependency assemblies are loaded into the default context, and the new context contains only the assemblies explicitly loaded into it.
        }
    }
}
