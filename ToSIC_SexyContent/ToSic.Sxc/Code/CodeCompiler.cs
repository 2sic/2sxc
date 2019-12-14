using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web.Compilation;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Code
{
    internal class CodeCompiler: HasLog
    {
        internal CodeCompiler(ILog parentLog) : base("Sys.CsCmpl", parentLog)
        {
        }

        internal const string CsFileExtension = ".cs";
        internal const string SharedCodeRootPathKeyInCache = "SharedCodeRootPath";
        
        internal object InstantiateClass(string virtualPath, string className = null, string relativePath = null, bool throwOnError = true)
        {
            var wrapLog = Log.Call($"{virtualPath}, {className}, {throwOnError}");

            // Perform various checks on the path values
            var hasErrorMessage = CheckIfPathsOkAndCleanUp(ref virtualPath, relativePath);
            if (hasErrorMessage != null)
            {
                Log.Add($"Error: {hasErrorMessage}");
                wrapLog("failed");
                if(!throwOnError) throw new Exception(hasErrorMessage);
                return null;
            }

            // if no name provided, use the name which is the same as the file name
            className = className ?? Path.GetFileNameWithoutExtension(virtualPath) ?? "unknown";

            var assembly = BuildManager.GetCompiledAssembly(virtualPath);
            var compiledType = assembly.GetType(className, throwOnError, true);

            if (compiledType == null)
            {
                wrapLog($"didn't find type '{className}'; throw error: {throwOnError}");
                if (throwOnError)
                    throw new Exception("Error while creating class instance.");
                return null;
            }

            var instance = RuntimeHelpers.GetObjectValue(Activator.CreateInstance(compiledType));
            AttachRelativePath(virtualPath, instance);

            wrapLog($"found: {instance != null}");
            return instance;
        }

        /// <summary>
        /// Check the path and perform various corrections
        /// </summary>
        /// <param name="virtualPath">primary path to use</param>
        /// <param name="relativePath">optional second path to which the primary one would be attached to</param>
        /// <returns>null if all is ok, or an error message if not</returns>
        private string CheckIfPathsOkAndCleanUp(ref string virtualPath, string relativePath)
        {
            if (string.IsNullOrWhiteSpace(virtualPath))
                return "no path/name provided";

            // if path relative, merge with shared code path
            virtualPath = virtualPath.Replace("\\", "/");
            if (!virtualPath.StartsWith("/"))
            {
                Log.Add($"Trying to resolve relative path: '{virtualPath}' using '{relativePath}'");
                if (relativePath == null)
                    return "Unexpected null value on relativePath";

                // if necessary, add trailing slash
                if (!relativePath.EndsWith("/"))
                    relativePath += "/";

                virtualPath = System.Web.VirtualPathUtility.Combine(relativePath, virtualPath);
                Log.Add($"final virtual path: '{virtualPath}'");
            }

            if (virtualPath.IndexOf(":", StringComparison.InvariantCultureIgnoreCase) > -1)
                return $"Tried to get .cs file, but found '{virtualPath}' containing ':', (not allowed)";

            return null;
        }


        private static void AttachRelativePath(string virtualPath, object instance)
        {
            // in case it supports shared code again, give it the relative path
            if (instance is ISharedCodeBuilder codeForwarding)
                codeForwarding.SharedCodeVirtualRoot = Path.GetDirectoryName(virtualPath);
        }
    }
}
