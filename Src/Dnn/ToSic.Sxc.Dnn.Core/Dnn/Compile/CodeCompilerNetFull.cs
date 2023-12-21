using System;
using System.IO;
using System.Web.Compilation;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Help;

namespace ToSic.Sxc.Dnn.Compile;

[PrivateApi]
internal class CodeCompilerNetFull(IServiceProvider serviceProvider, IRoslynBuildManager roslynBuildManager, Lazy<SourceAnalyzer> sourceAnalyzer) : CodeCompiler(serviceProvider)
{


    protected internal override AssemblyResult GetAssembly(string relativePath, string className, int appId = 0)
    {
        try
        {
            // TODO: SHOULD OPTIMIZE so the file doesn't need to read multiple times
            // 1. probably change so the CodeFileInfo contains the source code
            var razorType = sourceAnalyzer.Value.TypeOfVirtualPath(relativePath);

            if (razorType.MyApp) return roslynBuildManager.GetCompiledAssembly(relativePath, className, appId);
            return new AssemblyResult(BuildManager.GetCompiledAssembly(relativePath));
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