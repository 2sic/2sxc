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
        public CodeCompilerNetFull(IServiceProvider serviceProvider) : base(serviceProvider)
        { }

        protected override (Assembly Assembly, string ErrorMessages) GetAssembly(string relativePath, string className)
        {
            var assembly = BuildManager.GetCompiledAssembly(relativePath);
            return (assembly, null);
        }

        protected override (Type Type, string ErrorMessage) GetCsHtmlType(string relativePath)
        {
            var compiledType = BuildManager.GetCompiledType(relativePath);
            var errMsg = compiledType == null
                ? $"Couldn't create instance of {relativePath}. Compiled type == null" : null;
            return (compiledType, errMsg);
        }
    }
}