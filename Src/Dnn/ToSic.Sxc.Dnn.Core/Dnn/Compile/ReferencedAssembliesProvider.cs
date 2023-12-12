using System.Linq;
using System.Reflection;
using System.Web.Compilation;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Dnn.Compile
{
    [InternalApi_DoNotUse_MayChangeWithoutNotice()]
    public class ReferencedAssembliesProvider : IReferencedAssembliesProvider
    {
        public string[] Locations() => 
            (from Assembly assembly in BuildManager.GetReferencedAssemblies() select assembly.Location).ToArray();
    }
}
