using System.IO;
using System.Reflection;

namespace ToSic.Sxc.Dnn.Compile.AppDomain;

[PrivateApi]
internal abstract class AssemblyLoader : MarshalByRefObject
{
    protected Type LoadAssemblyAndGetTypeInfo(string assemblyPath, string typeName)
    {
        try
        {
            // do not use Assembly.LoadFrom() as it will lock the file
            var assembly = Assembly.Load(File.ReadAllBytes(assemblyPath));
            return assembly.GetType(typeName);
        }
        catch
        {
            // Handle exceptions appropriately
            return null;
        }
    }
}