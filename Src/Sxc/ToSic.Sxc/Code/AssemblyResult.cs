using System.Reflection;

namespace ToSic.Sxc.Code
{
    public class AssemblyResult
    {
        public Assembly Assembly { get; }
        public byte[] AssemblyBytes { get; }
        public string ErrorMessages { get; }

        public AssemblyResult(Assembly assembly = null, byte[] assemblyBytes = null, string errorMessages = null)
        {
            Assembly = assembly;
            AssemblyBytes = assemblyBytes;
            ErrorMessages = errorMessages;
        }
    }
}
