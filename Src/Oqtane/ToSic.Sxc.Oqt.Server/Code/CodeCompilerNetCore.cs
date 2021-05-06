using System;
using System.IO;
using System.Reflection;
using ToSic.Eav.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Oqt.Server.Code
{
    [PrivateApi]
    public class CodeCompilerNetCore: CodeCompiler
    {
        public CodeCompilerNetCore(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            
        }

        protected override Type GetCsHtmlType(string virtualPath) 
            => throw new Exception("Runtime Compile of .cshtml is Not Implemented in .net standard / core");
        protected override Assembly GetAssembly(string virtualPath, string className)
        {
            var fullPath = ServiceProvider.Build<IServerPaths>().FullContentPath(virtualPath.Backslash());
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
    }
}
