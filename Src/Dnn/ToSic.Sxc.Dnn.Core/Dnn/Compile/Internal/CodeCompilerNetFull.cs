﻿using System.Web.Compilation;
using ToSic.Eav.Apps.Sys.AppJson;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Code.Internal.SourceCode;

namespace ToSic.Sxc.Dnn.Compile.Internal;

[PrivateApi]
internal class CodeCompilerNetFull(
    IServiceProvider serviceProvider,
    IRoslynBuildManager roslynBuildManager,
    LazySvc<SourceAnalyzer> sourceAnalyzer,
    LazySvc<IAppJsonConfigurationService> appJson)
    : CodeCompiler(serviceProvider, connect: [roslynBuildManager, sourceAnalyzer, appJson])
{
    public override AssemblyResult GetAssembly(string relativePath, string className, HotBuildSpec spec)
    {
        var l = Log.Fn<AssemblyResult>($"{nameof(relativePath)}: '{relativePath}'; {nameof(className)}: '{className}'; {spec}", timer: true);

        AssemblyResult ReportError(Exception ex, string additionalInfo)
        {
            var errorMessage =
                $"Can't compile '{className}' in {Path.GetFileName(relativePath)}. Details are logged into insights. {additionalInfo}" +
                ex.Message;
            return new(errorMessages: errorMessage);
        }

        try
        {
            // TODO: SHOULD OPTIMIZE so the file doesn't need to read multiple times
            var codeFileInfo = sourceAnalyzer.Value.TypeOfVirtualPath(relativePath);

            try
            {
                if (appJson.Value.DnnCompilerAlwaysUseRoslyn(spec.AppId) || codeFileInfo.IsHotBuildSupported())
                    return l.Return(roslynBuildManager.GetCompiledAssembly(codeFileInfo, className, spec),
                        "Ok, RoslynBuildManager");
            }
            catch (Exception ex)
            {
                return l.ReturnAsError(ReportError(ex, "using Roslyn compiler"));
            }

            try
            {
                return l.Return(new(BuildManager.GetCompiledAssembly(relativePath)), "Ok, BuildManager");
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