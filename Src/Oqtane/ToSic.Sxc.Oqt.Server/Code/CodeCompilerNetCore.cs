using System;
using System.IO;
using System.Reflection;
using ToSic.Eav.Helpers;
using ToSic.Lib.Logging;
using ToSic.Eav.Run;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
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
        public override (Assembly Assembly, string ErrorMessages) GetAssembly(string virtualPath, string className)
        {
            // TODO: stv# compile from folder, etc...
            
            var l = Log.Fn<(Assembly Assembly, string ErrorMessages)>(
                $"{nameof(virtualPath)}: '{virtualPath}'; {nameof(className)}: '{className}'");
            var fullContentPath = _serverPaths.Value.FullContentPath(virtualPath.Backslash());
            var fullPath = NormalizeFullPath(fullContentPath);
            l.A($"New paths: '{fullContentPath}', '{fullPath}'");
            try
            {
                var assembly = new Compiler().Compile(fullPath, className);
                return l.Return((assembly, null), "ok");
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                var errorMessage =
                    $"Error: Can't compile '{className}' in {Path.GetFileName(virtualPath)}. Details are logged into insights. " +
                    ex.Message;
                return l.Return((null, errorMessage), "error");
            }
        }
    }
}
