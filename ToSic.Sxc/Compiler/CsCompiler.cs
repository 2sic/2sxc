using System;
using System.Runtime.CompilerServices;
using System.Web;
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
        internal const string SharedCodeRootPathKeyInCache = "SharedCodeRootPath";
        
        internal object InstantiateClass(string virtualPath, string name = null, bool throwOnError = true)
        {
            var wrapLog = Log.Call("InstantiateClass", $"{virtualPath}, {name}, {throwOnError}");

            if (string.IsNullOrWhiteSpace(virtualPath))
            {
                wrapLog("no path given");
                if (throwOnError) throw new Exception("no name provided");
                return null;
            }

            //var fileName = name;
            if (virtualPath.IndexOf(":", StringComparison.InvariantCultureIgnoreCase) > -1)
            {
                Log.Add($"path contains ':': {virtualPath}");
                wrapLog("failed");
                throw new Exception("Tried to get .cs file to compile, but found a full path, which is not allowed.");
                //var root = HttpContext.Current.Server.MapPath("~/");
                //var rootOtherSlash = root.Replace("/", @"\");
                //if (virtualPath.StartsWith(root, StringComparison.InvariantCultureIgnoreCase) ||
                //    virtualPath.StartsWith(rootOtherSlash, StringComparison.InvariantCultureIgnoreCase))
                //    virtualPath = virtualPath.Remove(0, root.Length - 1);
                //else
                //{
                //    var msg = $"Path '{virtualPath}' contains ':' but not from application root '{root}', will cancel.";
                //    Log.Add(msg);
                //    wrapLog("failed");
                //    throw new Exception(msg);
                //}
            }
            name = name ?? System.IO.Path.GetFileNameWithoutExtension(virtualPath);

            var assembly = BuildManager.GetCompiledAssembly(virtualPath);
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
