using System;
using System.Reflection;
using System.Web.Compilation;
using ToSic.Lib.Documentation;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Code
{
    [PrivateApi]
    public class CodeCompilerNetFull: CodeCompiler
    {
        protected override (Assembly Assembly, string ErrorMessages) GetAssembly(string virtualPath, string className)
        {
            var assembly = BuildManager.GetCompiledAssembly(virtualPath);
            return (assembly, null);
        }

        protected override (Type Type, string ErrorMessage) GetCsHtmlType(string virtualPath)
        {
            var compiledType = BuildManager.GetCompiledType(virtualPath);
            var errMsg = (compiledType == null)
                ? $"Couldn't create instance of {virtualPath}. Compiled type == null" : null;
            return (compiledType, errMsg);
        }
    }
}