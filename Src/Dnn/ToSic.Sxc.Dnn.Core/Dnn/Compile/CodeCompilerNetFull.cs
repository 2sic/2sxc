using System;
using System.Reflection;
using System.Web.Compilation;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Dnn.Compile
{
    [PrivateApi]
    internal class CodeCompilerNetFull: CodeCompiler
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