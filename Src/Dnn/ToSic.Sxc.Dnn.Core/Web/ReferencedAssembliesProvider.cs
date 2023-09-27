using System.Linq;
using System.Reflection;
using System.Web.Compilation;

namespace ToSic.Sxc.Web
{
    public class ReferencedAssembliesProvider : IReferencedAssembliesProvider
    {
        public string[] Locations() => 
            (from Assembly assembly in BuildManager.GetReferencedAssemblies() select assembly.Location).ToArray();
    }
}
