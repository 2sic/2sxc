using System;
using System.Runtime.CompilerServices;
using System.Web.Compilation;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;

namespace ToSic.Sxc.Compiler
{
    internal class CsCompiler: HasLog
    {
        internal CsCompiler(Log parentLog) : base("Sys.CsCmpl", parentLog)
        {
        }

        internal const string CsFileExtension = ".cs";
        // instantiate class
        internal object InstantiateClass(string path, string name = null, bool throwOnError = true)
        {
            var wrapLog = Log.Call("InstantiateClass", $"{path}, {name}, {throwOnError}");

            if (string.IsNullOrWhiteSpace(path))
            {
                wrapLog("no path given");
                if (throwOnError) throw new Exception("no name provided");
                return null;
            }

            //var fileName = name;
            //var path = System.IO.Path.Combine(fileName);
            name = name ?? System.IO.Path.GetFileNameWithoutExtension(path);

            var assembly = BuildManager.GetCompiledAssembly(path);
            var compiledType = assembly.GetType(name, throwOnError, true);

            if (compiledType == null)
            {
                wrapLog($"didn't find type '{name}'; throw error: {throwOnError}");
                if (throwOnError)
                    throw new Exception("Error while creating class instance.");
                return null;
            }

            var instance = RuntimeHelpers.GetObjectValue(Activator.CreateInstance(compiledType));
            wrapLog($"found: {instance != null}");
            return instance;
        }

    }
}
