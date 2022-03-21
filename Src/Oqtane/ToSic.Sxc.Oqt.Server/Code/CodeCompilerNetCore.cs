using System;
using System.IO;
using System.Reflection;
using ToSic.Eav.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Eav.Run;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Oqt.Server.Code
{
    [PrivateApi]
    public class CodeCompilerNetCore: CodeCompiler
    {
        private readonly Lazy<IServerPaths> _serverPaths;

        public CodeCompilerNetCore(Lazy<IServerPaths> serverPaths)
        {
            _serverPaths = serverPaths;
        }

        protected override Type GetCsHtmlType(string virtualPath) 
            => throw new Exception("Runtime Compile of .cshtml is Not Implemented in .net standard / core");
        protected override Assembly GetAssembly(string virtualPath, string className)
        {
            var fullPath = _serverPaths.Value.FullContentPath(virtualPath.Backslash());
            fullPath = NormalizeFullFilePath(fullPath);
            try
            {
                return new Compiler().Compile(fullPath, className);
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                ErrorMessage =
                    $"Error: Can't compile '{className}' in {Path.GetFileName(virtualPath)}. Details are logged into insights. " +
                    ex.Message;
            }

            return null;
        }

        /**
         * Normalize full file path, so it is without redirections like "../" in "dir1/dir2/../file.cs"
         */
        public static string NormalizeFullFilePath(string fullPath)
        {
            return new FileInfo(fullPath).FullName;
        }
    }
}
