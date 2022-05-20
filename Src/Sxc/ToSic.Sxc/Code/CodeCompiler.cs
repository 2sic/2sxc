using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using ToSic.Eav.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Code
{
    [PrivateApi]
    public abstract class CodeCompiler: HasLog<CodeCompiler>
    {
        #region Constructor / DI

        internal CodeCompiler() : base("Sys.CsCmpl")
        { }

        #endregion

        public const string CsFileExtension = ".cs";
        public const string CsHtmlFileExtension = ".cshtml";
        public const string SharedCodeRootPathKeyInCache = "SharedCodeRootPath";

        protected string ErrorMessage;

        internal object InstantiateClass(string virtualPath, string className = null, string relativePath = null, bool throwOnError = true)
        {
            var wrapLog = Log.Call($"{virtualPath}, {nameof(className)}:{className}, {nameof(relativePath)}:{relativePath}, {throwOnError}");
            //string ErrorMessage = null;

            // Perform various checks on the path values
            var hasErrorMessage = CheckIfPathsOkAndCleanUp(ref virtualPath, relativePath);
            if (hasErrorMessage != null)
            {
                Log.A($"Error: {hasErrorMessage}");
                wrapLog("failed");
                if (throwOnError) throw new Exception(hasErrorMessage);
                return null;
            }

            var pathLowerCase = virtualPath.ToLowerInvariant();
            var isCs = pathLowerCase.EndsWith(CsFileExtension);
            var isCshtml = pathLowerCase.EndsWith(CsHtmlFileExtension);

            Type compiledType = null;
            if (isCshtml && string.IsNullOrEmpty(className))
            {
                compiledType = GetCsHtmlType(virtualPath);
            }
            // compile .cs files
            else if (isCs || isCshtml)
            {
                // if no name provided, use the name which is the same as the file name
                className = className ?? Path.GetFileNameWithoutExtension(virtualPath) ?? "unknown";

                var assembly = GetAssembly(virtualPath, className);

                if (ErrorMessage == null)
                {
                    var possibleErrorMessage =
                        $"Error: Didn't find type '{className}' in {Path.GetFileName(virtualPath)}. Maybe the class name doesn't match the file name. ";
                    try
                    {
                        compiledType = assembly?.GetType(className, throwOnError, true);
                    }
                    catch (Exception ex)
                    {
                        Log.A(possibleErrorMessage);
                        if(throwOnError) throw new TypeLoadException(possibleErrorMessage, ex);
                    }

                    if (compiledType == null)
                        ErrorMessage = possibleErrorMessage;
                }
            }
            else
                ErrorMessage = $"Error: given path '{Path.GetFileName(virtualPath)}' doesn't point to a .cs or .cshtml";

            if (ErrorMessage != null)
            {
                Log.A(ErrorMessage + $"; throw error: {throwOnError}");
                wrapLog("failed");
                if (throwOnError) throw new Exception(ErrorMessage);
                return null;
            }

            var instance = RuntimeHelpers.GetObjectValue(Activator.CreateInstance(compiledType));
            AttachRelativePath(virtualPath, instance);

            wrapLog($"found: {instance != null}");
            return instance;

        }

        protected abstract Assembly GetAssembly(string virtualPath, string className);


        protected abstract Type GetCsHtmlType(string virtualPath);


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
            virtualPath = virtualPath.ForwardSlash();// .Replace("\\", "/");
            if (!virtualPath.StartsWith("/"))
            {
                Log.A($"Trying to resolve relative path: '{virtualPath}' using '{relativePath}'");
                if (relativePath == null)
                    return "Unexpected null value on relativePath";

                // if necessary, add trailing slash
                //if (!relativePath.EndsWith("/"))
                relativePath = relativePath.SuffixSlash();// += "/";
                //virtualPath = _serviceProvider.Build<ILinkPaths>().ToAbsolute(Path.Combine(relativePath, virtualPath));
                virtualPath = Path.Combine(relativePath, virtualPath).ToAbsolutePathForwardSlash();
                Log.A($"final virtual path: '{virtualPath}'");
            }

            if (virtualPath.IndexOf(":", StringComparison.InvariantCultureIgnoreCase) > -1)
                return $"Tried to get .cs file, but found '{virtualPath}' containing ':', (not allowed)";

            return null;
        }


        private bool AttachRelativePath(string virtualPath, object instance)
        {
            var wrapLog = Log.Fn<bool>();

            if (!(instance is ICreateInstance codeForwarding)) 
                return wrapLog.Return(false, "didn't attach");

            // in case it supports shared code again, give it the relative path
            codeForwarding.CreateInstancePath = Path.GetDirectoryName(virtualPath);
            return wrapLog.Return(true, "attached");
        }
    }
}
