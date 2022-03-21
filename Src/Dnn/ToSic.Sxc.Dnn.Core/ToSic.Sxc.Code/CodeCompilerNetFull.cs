using System;
using System.Reflection;
using System.Web.Compilation;
using ToSic.Eav.Documentation;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Code
{
    [PrivateApi]
    public class CodeCompilerNetFull: CodeCompiler
    {
        protected override Assembly GetAssembly(string virtualPath, string className)
        {
            var assembly = BuildManager.GetCompiledAssembly(virtualPath);
            return assembly;
        }

        protected override Type GetCsHtmlType(string virtualPath)
        {
            var compiledType = BuildManager.GetCompiledType(virtualPath);
            if (compiledType == null)
                ErrorMessage = $"Couldn't create instance of {virtualPath}. Compiled type == null";
            return compiledType;
        }
    }
}