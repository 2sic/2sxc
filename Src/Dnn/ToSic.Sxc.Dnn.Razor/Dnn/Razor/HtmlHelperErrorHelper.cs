using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Code.Sys.CodeErrorHelp;
using ToSic.Sxc.Code.Sys.SourceCode;
using ToSic.Sxc.Render.Sys;
using ToSic.Sxc.Sys.Configuration;

namespace ToSic.Sxc.Dnn.Razor;
internal class HtmlHelperErrorHelper(RazorComponentBase page, bool isSystemAdmin, DnnRazorHelper helper,
    LazySvc<IFeaturesService> featureSvc, LazySvc<SourceAnalyzer> codeAnalysis, LazySvc<CodeErrorHelpService> codeErrService, Generator<IRenderingHelper> renderingHelperGenerator)
{
    internal HashSet<string> ErrorPaths = new(StringComparer.InvariantCultureIgnoreCase);

    internal string TryToLogAndReWrapError(Exception renderException, string path, bool reportToDnn, string additionalLog = null)
    {
        // Important to know: Once this fires, the page will stop rendering more templates
        if (reportToDnn)
            page.Log.GetContents().Ex(renderException);
        if (additionalLog != null)
            page.Log.GetContents().A(additionalLog);

        // If it's a compile issue, try to find explicit help for that
        var pathOfPage = page.NormalizePath(path);
        var razorType = codeAnalysis.Value.TypeOfVirtualPath(pathOfPage);
        var exWithHelp = codeErrService.Value.AddHelpForCompileProblems(renderException, razorType);


        // Show a nice / ugly error depending on user permissions
        // Note that if anything breaks here, it will just use the normal error - but for what breaks in here
        // Note that if withHelp already has help, it won't be extended anymore
        exWithHelp = codeErrService.Value.AddHelpIfKnownError(exWithHelp, page);
        var block = page.ExCtx.GetState<IBlock>();
        var renderHelper = renderingHelperGenerator.New().Init(block);
        var nice = renderHelper.DesignErrorMessage([exWithHelp], true);
        helper.Add(exWithHelp);
        return nice;
    }


    internal bool ThrowPartialError => _throwPartialError.Get(()
        => featureSvc.Value.IsEnabled(SxcFeatures.RazorThrowPartial.NameId) ||
           isSystemAdmin && featureSvc.Value.IsEnabled(SxcFeatures.RenderThrowPartialSystemAdmin.NameId));
    private readonly GetOnce<bool> _throwPartialError = new();
}
