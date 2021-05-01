using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Sxc.Run;
#if NET451
using System.Web.Compilation;
#endif
#if NETSTANDARD
using ToSic.Sxc.Code.Builder;
#endif


namespace ToSic.Sxc.Code
{
    public class CodeCompiler: HasLog
    {
        private readonly IServiceProvider _serviceProvider;

        #region Constructor / DI

        internal CodeCompiler(IServiceProvider serviceProvider, ILog parentLog) : base("Sys.CsCmpl", parentLog)
        {
            _serviceProvider = serviceProvider;
        }

        #endregion

        public const string CsFileExtension = ".cs";
        public const string CsHtmlFileExtension = ".cshtml";
        public const string SharedCodeRootPathKeyInCache = "SharedCodeRootPath";

        internal object InstantiateClass(string virtualPath, string className = null, string relativePath = null, bool throwOnError = true)
        {
            var wrapLog = Log.Call($"{virtualPath}, {nameof(className)}:{className}, {nameof(relativePath)}:{relativePath}, {throwOnError}");
            string errorMsg = null;

            // Perform various checks on the path values
            var hasErrorMessage = CheckIfPathsOkAndCleanUp(ref virtualPath, relativePath);
            if (hasErrorMessage != null)
            {
                Log.Add($"Error: {hasErrorMessage}");
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
#if NETSTANDARD
                throw new Exception("Runtime Compile of .cshtml is Not Implemented in .net standard / core");
#else
                compiledType = BuildManager.GetCompiledType(virtualPath);
                if (compiledType == null)
                    errorMsg = $"Couldn't create instance of {virtualPath}. Compiled type == null";
#endif
            }
            // compile .cs files
            else if (isCs || isCshtml)
            {
                // if no name provided, use the name which is the same as the file name
                className = className ?? Path.GetFileNameWithoutExtension(virtualPath) ?? "unknown";

                Assembly assembly = null;
#if NETSTANDARD
                var fullPath = _serviceProvider.Build<IServerPaths>().FullContentPath(virtualPath.Backslash());
                try
                {
                    assembly = new Compiler().Compile(fullPath, className);
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    errorMsg = $"Error: Can't compile '{className}' in {Path.GetFileName(virtualPath)}. Details are logged into insights. " + ex.Message;
                }
#else
                assembly = BuildManager.GetCompiledAssembly(virtualPath);
#endif
                if (errorMsg == null)
                {
                    var possibleErrorMessage =
                        $"Error: Didn't find type '{className}' in {Path.GetFileName(virtualPath)}. Maybe the class name doesn't match the file name. ";
                    try
                    {
                        compiledType = assembly?.GetType(className, throwOnError, true);
                    }
                    catch (Exception ex)
                    {
                        Log.Add(possibleErrorMessage);
                        if(throwOnError) throw new TypeLoadException(possibleErrorMessage, ex);
                    }

                    if (compiledType == null)
                        errorMsg = possibleErrorMessage;
                }
            }
            else
                errorMsg = $"Error: given path '{Path.GetFileName(virtualPath)}' doesn't point to a .cs or .cshtml";

            if (errorMsg != null)
            {
                Log.Add(errorMsg + $"; throw error: {throwOnError}");
                wrapLog("failed");
                if (throwOnError) throw new Exception(errorMsg);
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
            virtualPath = virtualPath.ForwardSlash();// .Replace("\\", "/");
            if (!virtualPath.StartsWith("/"))
            {
                Log.Add($"Trying to resolve relative path: '{virtualPath}' using '{relativePath}'");
                if (relativePath == null)
                    return "Unexpected null value on relativePath";

                // if necessary, add trailing slash
                //if (!relativePath.EndsWith("/"))
                relativePath = relativePath.SuffixSlash();// += "/";
                //virtualPath = _serviceProvider.Build<ILinkPaths>().ToAbsolute(Path.Combine(relativePath, virtualPath));
                virtualPath = Path.Combine(relativePath, virtualPath).ToAbsolutePathForwardSlash();
                Log.Add($"final virtual path: '{virtualPath}'");
            }

            if (virtualPath.IndexOf(":", StringComparison.InvariantCultureIgnoreCase) > -1)
                return $"Tried to get .cs file, but found '{virtualPath}' containing ':', (not allowed)";

            return null;
        }


        private bool AttachRelativePath(string virtualPath, object instance)
        {
            var wrapLog = Log.Call<bool>();
            // in case it supports shared code again, give it the relative path
            if (instance is ICreateInstance codeForwarding)
            {
                codeForwarding.CreateInstancePath = Path.GetDirectoryName(virtualPath);
                return wrapLog("attached", true);
            }
            return wrapLog("didn't attach", false);
        }
    }
}
