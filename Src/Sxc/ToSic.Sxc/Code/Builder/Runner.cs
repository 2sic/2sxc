#if NETSTANDARD
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ToSic.Sxc.Code.Builder
{
    public class Runner
    {
        // Ensure that can't be kept alive by stack slot references (real- or JIT-introduced locals).
        // That could keep the SimpleUnloadableAssemblyLoadContext alive and prevent the unload.
        [MethodImpl(MethodImplOptions.NoInlining)]
        public Assembly Load(byte[] compiledAssembly)
        {
            using (var asm = new MemoryStream(compiledAssembly))
            {
                var assemblyLoadContext = new SimpleUnloadableAssemblyLoadContext();
                return assemblyLoadContext.LoadFromStream(asm);
            }
        }
    }
}
#endif
