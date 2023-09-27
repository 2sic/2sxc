//using System.Collections.Generic;
//using System.IO;
//using System.Reflection;
//using System.Runtime.Loader;

//namespace ToSic.Sxc.Oqt.Server.Code
//{
//    public class MemoryLoadContext : AssemblyLoadContext
//    {
//        private readonly Dictionary<string, byte[]> _assemblies;

//        public MemoryLoadContext(Dictionary<string, byte[]> assemblies)
//        {
//            _assemblies = assemblies;
//        }

//        protected override Assembly Load(AssemblyName assemblyName)
//        {
//            if (_assemblies.TryGetValue(assemblyName.FullName, out var assemblyData))
//            {
//                return LoadFromStream(new MemoryStream(assemblyData));
//            }

//            return null;
//        }
//    }
//}
