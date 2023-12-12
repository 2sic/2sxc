using System;
using System.IO;
using System.Web.Compilation;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Dnn.Compile;

[PrivateApi]
internal class CodeCompilerNetFull(IServiceProvider serviceProvider) : CodeCompiler(serviceProvider)
{
    protected internal override AssemblyResult GetAssembly(string relativePath, string className, int appId = 0)
    {
        var l = Log.Fn<AssemblyResult>(
            $"{nameof(relativePath)}: '{relativePath}'; {nameof(className)}: '{className}'; {nameof(appId)}: {appId}");

        try
        {
            var assembly = BuildManager.GetCompiledAssembly(relativePath);
            return l.ReturnAsOk(new AssemblyResult(assembly));
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            var errorMessage =
                $"Error: Can't compile '{className}' in {Path.GetFileName(relativePath)}. Details are logged into insights. " +
                ex.Message;
            return l.ReturnAsError(new AssemblyResult(errorMessages: errorMessage), "error");
        }
    }

    protected override (Type Type, string ErrorMessage) GetCsHtmlType(string relativePath)
    {
        var l = Log.Fn<(Type Type, string ErrorMessage)>($"{nameof(relativePath)}: '{relativePath}'", timer: true);
        var compiledType = BuildManager.GetCompiledType(relativePath);
        var errMsg = compiledType == null
            ? $"Couldn't create instance of {relativePath}. Compiled type == null" : null;
        return l.Return((compiledType, errMsg), errMsg ?? "Ok");
    }
}