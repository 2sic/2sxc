using System.Reflection;

namespace ToSic.Sxc.Code
{
    public class AssemblyResult
    {
        public Assembly Assembly { get; }
        public byte[] AssemblyBytes { get; }
        public string ErrorMessages { get; }
        public string[] AssemblyLocations { get; }
        public string SafeClassName { get; }


        public AssemblyResult(Assembly assembly = null, byte[] assemblyBytes = null, string errorMessages = null, string[] assemblyLocations = null, string safeClassName = null)
        {
            Assembly = assembly;
            AssemblyBytes = assemblyBytes;
            ErrorMessages = errorMessages;
            AssemblyLocations = assemblyLocations;
            SafeClassName = safeClassName;
        }
    }
}
