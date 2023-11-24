using System;
using System.IO;
using System.Reflection;
using ToSic.Lib.Documentation;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Code
{
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
}