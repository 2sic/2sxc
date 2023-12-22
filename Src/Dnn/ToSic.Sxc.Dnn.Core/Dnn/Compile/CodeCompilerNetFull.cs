using System;
using System.IO;
using System.Web.Compilation;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Help;

namespace ToSic.Sxc.Dnn.Compile;

[PrivateApi]
internal class CodeCompilerNetFull : CodeCompiler
{
    private readonly IRoslynBuildManager _roslynBuildManager;
    private readonly LazySvc<SourceAnalyzer> _sourceAnalyzer;

    public CodeCompilerNetFull(IServiceProvider serviceProvider, IRoslynBuildManager roslynBuildManager, LazySvc<SourceAnalyzer> sourceAnalyzer) : base(serviceProvider)
    {
        ConnectServices(
            _roslynBuildManager = roslynBuildManager,
            _sourceAnalyzer = sourceAnalyzer
        );
    }

    protected internal override AssemblyResult GetAssembly(string relativePath, string className, int appId = 0)
    {
        var l = Log.Fn<AssemblyResult>($"{nameof(relativePath)}:{relativePath}, {nameof(className)}:{className}, {nameof(appId)}:{appId}", timer: true);

        AssemblyResult ReportError(Exception ex, string additionalInfo)
        {
            var errorMessage =
                $"Can't compile '{className}' in {Path.GetFileName(relativePath)}. Details are logged into insights. {additionalInfo}" +
                ex.Message;
            return new AssemblyResult(errorMessages: errorMessage);
        }

        try
        {
            // TODO: SHOULD OPTIMIZE so the file doesn't need to read multiple times
            // 1. probably change so the CodeFileInfo contains the source code
            var code = _sourceAnalyzer.Value.TypeOfVirtualPath(relativePath);

            try
            {
                if (code.ThisApp)
                    return l.Return(_roslynBuildManager.GetCompiledAssembly(relativePath, className, appId),
                        "Ok, RoslynBuildManager");
            }
            catch (Exception ex)
            {
                return l.ReturnAsError(ReportError(ex, "using Roslyn compiler"));
            }

            try
            {
                return l.Return(new AssemblyResult(BuildManager.GetCompiledAssembly(relativePath)), "Ok, BuildManager");
            }
            catch (Exception ex)
            {
                return l.ReturnAsError(ReportError(ex, "using BuildManager"));
            }
        }
        catch (Exception ex)
        {
            return l.ReturnAsError(ReportError(ex, ""));
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