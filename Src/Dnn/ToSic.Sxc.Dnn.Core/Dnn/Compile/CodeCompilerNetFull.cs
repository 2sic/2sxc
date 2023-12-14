using System;
using System.IO;
using System.Web.Compilation;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Dnn.Compile;

[PrivateApi]
internal class CodeCompilerNetFull(IServiceProvider serviceProvider) : CodeCompiler(serviceProvider)
{
    protected internal override AssemblyResult GetAssembly(string relativePath, string className, int appId = 0)
    {
        try
        {
            var assembly = BuildManager.GetCompiledAssembly(relativePath);
            return new AssemblyResult(assembly);
        }
        catch (Exception ex)
        {
            var errorMessage =
                $"Error: Can't compile '{className}' in {Path.GetFileName(relativePath)}. Details are logged into insights. " +
                ex.Message;
            return new AssemblyResult(errorMessages: errorMessage);
        }
    }

    protected override (Type Type, string ErrorMessage) GetCsHtmlType(string relativePath)
    {
        var compiledType = BuildManager.GetCompiledType(relativePath);
        var errMsg = compiledType == null
            ? $"Couldn't create instance of {relativePath}. Compiled type == null" : null;
        return (compiledType, errMsg);
    }
}