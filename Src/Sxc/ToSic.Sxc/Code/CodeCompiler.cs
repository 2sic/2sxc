using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ToSic.Eav.Helpers;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using static System.StringComparison;

namespace ToSic.Sxc.Code
{
    [PrivateApi]
    public abstract class CodeCompiler: ServiceBase
    {
        private readonly IServiceProvider _serviceProvider;

        #region Constructor / DI

        internal CodeCompiler(IServiceProvider serviceProvider) : base("Sys.CsCmpl")
        {
            _serviceProvider = serviceProvider;
        }

        #endregion

        public const string CsFileExtension = ".cs";
        public const string CsHtmlFileExtension = ".cshtml";
        public const string SharedCodeRootPathKeyInCache = "SharedCodeRootPath";

        internal object InstantiateClass(string virtualPath, string className = null, string relativePath = null, bool throwOnError = true)
        {
            var l = Log.Fn<object>($"{virtualPath}, {nameof(className)}:{className}, {nameof(relativePath)}:{relativePath}, {throwOnError}");

            // Perform various checks on the path values
            var hasErrorMessage = CheckIfPathsOkAndCleanUp(ref virtualPath, relativePath);
            if (hasErrorMessage != null)
            {
                l.A($"Error: {hasErrorMessage}");
                l.ReturnNull("failed");
                if (throwOnError) throw new Exception(hasErrorMessage);
                return null;
            }

            var pathLowerCase = virtualPath.ToLowerInvariant();
            var isCs = pathLowerCase.EndsWith(CsFileExtension);
            var isCshtml = pathLowerCase.EndsWith(CsHtmlFileExtension);

            Type compiledType = null;
            string errorMessages;
            if (isCshtml && string.IsNullOrEmpty(className))
                (compiledType, errorMessages) = GetCsHtmlType(virtualPath);
            // compile .cs files
            else if (isCs || isCshtml)
                (compiledType, errorMessages) = GetTypeOrErrorMessages(virtualPath, className, throwOnError);
            else
                errorMessages = $"Error: given path '{Path.GetFileName(virtualPath)}' doesn't point to a .cs or .cshtml";

            if (errorMessages != null)
            {
                l.A($"{errorMessages}; throw error: {throwOnError}");
                l.ReturnNull("failed");
                if (throwOnError) throw new Exception(errorMessages);
                return null;
            }

            var instance = _serviceProvider.Build<object>(compiledType, Log);
            AttachRelativePath(virtualPath, instance);
            
            return l.Return(instance, $"found: {instance != null}");
        }

        public (Type Type, string ErrorMessages) GetTypeOrErrorMessages(string relativePath, string className, bool throwOnError)
        {
            var l = Log.Fn<(Type Type, string ErrorMessages)>($"'{relativePath}', '{className}', throw: {throwOnError}");

            // if no name provided, use the name which is the same as the file name
            className = className ?? Path.GetFileNameWithoutExtension(relativePath) ?? Eav.Constants.NullNameId;

            var (assembly, errorMessages) = GetAssembly(relativePath, className);

            if (errorMessages != null) return l.Return((null, errorMessages), "error messages");

            if (assembly == null) return l.Return((null, "assembly is null"), "no assembly");

            var possibleErrorMessage = $"Error: Didn't find type '{className}' in {Path.GetFileName(relativePath)}. Maybe the class name doesn't match the file name. ";
            Type compiledType = null;
            try
            {
                compiledType = assembly.GetType(className, false, true);
                if (compiledType == null)
                {
                    var types = assembly.GetTypes();
                    compiledType = types.FirstOrDefault(t => t.Name.EqualsInsensitive(className));
                }

                if (compiledType == null && throwOnError)
                    assembly.GetType(className, true, true);
            }
            catch (Exception ex)
            {
                l.A(possibleErrorMessage);
                if (throwOnError) throw new TypeLoadException(possibleErrorMessage, ex);
            }

            if (compiledType == null)
                errorMessages = possibleErrorMessage;

            return l.Return((compiledType, errorMessages), errorMessages == null ? "ok" : "errors");
        }

        protected abstract (Assembly Assembly, string ErrorMessages) GetAssembly(string relativePath, string className);


        protected abstract (Type Type, string ErrorMessage) GetCsHtmlType(string relativePath);


        /// <summary>
        /// Check the path and perform various corrections
        /// </summary>
        /// <param name="virtualPath">primary path to use</param>
        /// <param name="relativePath">optional second path to which the primary one would be attached to</param>
        /// <returns>null if all is ok, or an error message if not</returns>
        private string CheckIfPathsOkAndCleanUp(ref string virtualPath, string relativePath)
        {
            var l = Log.Fn<string>($"{nameof(virtualPath)}: '{virtualPath}', {nameof(relativePath)}: '{relativePath}'");
            if (string.IsNullOrWhiteSpace(virtualPath))
                return l.ReturnAndLog("no path/name provided");

            // if path relative, merge with shared code path
            virtualPath = virtualPath.ForwardSlash();
            if (!virtualPath.StartsWith("/"))
            {
                l.A($"Trying to resolve relative path: '{virtualPath}' using '{relativePath}'");
                if (relativePath == null)
                    return l.ReturnAndLog("Unexpected null value on relativePath");

                // if necessary, add trailing slash
                relativePath = relativePath.SuffixSlash();
                virtualPath = Path.Combine(relativePath, virtualPath).ToAbsolutePathForwardSlash();
                l.A($"final virtual path: '{virtualPath}'");
            }

            if (virtualPath.IndexOf(":", InvariantCultureIgnoreCase) > -1)
                return l.ReturnAndLog($"Tried to get .cs file, but found '{virtualPath}' containing ':', (not allowed)");

            return l.ReturnNull("all ok");
        }


        private bool AttachRelativePath(string virtualPath, object instance)
        {
            var l = Log.Fn<bool>();

            if (!(instance is ICreateInstance codeForwarding)) 
                return l.ReturnFalse("didn't attach");

            // in case it supports shared code again, give it the relative path
            codeForwarding.CreateInstancePath = Path.GetDirectoryName(virtualPath);
            return l.ReturnTrue("attached");
        }
    }
}
