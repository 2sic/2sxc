using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.IO;
using System.Linq;
using ToSic.Eav.Helpers;
using ToSic.Eav.Run;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Oqt.Server.Code
{
    [PrivateApi]
    public class CodeCompilerNetCore: CodeCompiler
    {
        private readonly LazySvc<IServerPaths> _serverPaths;

        public CodeCompilerNetCore(LazySvc<IServerPaths> serverPaths, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            ConnectServices(
                _serverPaths = serverPaths
            );
        }

        protected override (Type Type, string ErrorMessage) GetCsHtmlType(string virtualPath) 
            => throw new("Runtime Compile of .cshtml is Not Implemented in .net standard / core");
        public override AssemblyResult GetAssembly(string virtualPath, string className = null)
        {
            // TODO: stv# compile from folder, etc...
            
            var l = Log.Fn<AssemblyResult>(
                $"{nameof(virtualPath)}: '{virtualPath}'; {nameof(className)}: '{className}'");
            var fullPath = GetFullPath(virtualPath);
            l.A($"New path: '{fullPath}'");
            try
            {
                var assemblyResult = new Compiler().Compile(fullPath, className ?? ConvertToSafeFileName(fullPath));
                return l.ReturnAsOk(assemblyResult);
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                var errorMessage =
                    $"Error: Can't compile '{className}' in {Path.GetFileName(virtualPath)}. Details are logged into insights. " +
                    ex.Message;
                return l.ReturnAsError(new AssemblyResult(errorMessages: errorMessage));
            }
        }

        public string GetFullPath(string virtualPath) => NormalizeFullPath(_serverPaths.Value.FullContentPath(virtualPath.Backslash()));

        private static string ConvertToSafeFileName(string fullPath)
        {
            // Get the invalid file name characters from the system
            var invalidChars = Path.GetInvalidFileNameChars();

            // Replace each invalid character with '_'
            return new string(fullPath.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray());
        }
    }
}
