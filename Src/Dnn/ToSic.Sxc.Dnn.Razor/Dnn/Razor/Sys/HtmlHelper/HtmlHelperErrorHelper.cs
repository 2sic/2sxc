using ToSic.Sxc.Code.Sys.CodeErrorHelp;
using ToSic.Sxc.Code.Sys.SourceCode;
using ToSic.Sxc.Render.Sys;
using ToSic.Sxc.Sys.Configuration;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Dnn.Razor.Sys;
internal class HtmlHelperErrorHelper(
    LazySvc<IFeaturesService> featureSvc,
    LazySvc<SourceAnalyzer> codeAnalysis,
    LazySvc<CodeErrorHelpService> codeErrService,
    Generator<IRenderingHelper> renderingHelperGenerator)
: ServiceWithSetup<HtmlHelperContextWithPaths>("Dnn.ErrHlp", connect: [featureSvc, codeAnalysis, codeErrService, renderingHelperGenerator])
{
    internal HashSet<string> ErrorPaths = new(StringComparer.InvariantCultureIgnoreCase);

    /// <summary>
    /// This exception is usually thrown when a thread is aborted, e.g. by Response.End in classic ASP.NET.
    /// It happens on Response.Redirect(...) calls.
    /// </summary>
    private const bool IgnoreThreadAbortException = true;

    internal string TryToLogAndReWrapError(Exception renderException, string path, bool reportToDnn, string additionalLog = null)
    {
        if (IgnoreThreadAbortException && renderException is ThreadAbortException)
            return "thread aborted; probably Response.Redirect called";

        // Important to know: Once this fires, the page will stop rendering more templates
        if (reportToDnn)
            MyOptions.Page.Log.GetContents().Ex(renderException);
        if (additionalLog != null)
            MyOptions.Page.Log.GetContents().A(additionalLog);

        // If it's a compile issue, try to find explicit help for that
        var razorType = codeAnalysis.Value.TypeOfVirtualPath(MyOptions.Normalized);
        var exWithHelp = codeErrService.Value.AddHelpForCompileProblems(renderException, razorType);


        // Show a nice / ugly error depending on user permissions
        // Note that if anything breaks here, it will just use the normal error - but for what breaks in here
        // Note that if withHelp already has help, it won't be extended anymore
        exWithHelp = codeErrService.Value.AddHelpIfKnownError(exWithHelp, MyOptions.Page);
        var block = MyOptions.ExCtx.GetBlock();
        var renderHelper = renderingHelperGenerator.New().Init(block);
        var nice = renderHelper.DesignErrorMessage([exWithHelp], true);
        MyOptions.RazorHelper.Add(exWithHelp);
        return nice;
    }


    internal bool ThrowPartialError => _throwPartialError.Get(()
        => featureSvc.Value.IsEnabled(SxcFeatures.RazorThrowPartial.NameId) ||
           MyOptions.IsSystemAdmin && featureSvc.Value.IsEnabled(SxcFeatures.RenderThrowPartialSystemAdmin.NameId));
    private readonly LazyGet<bool> _throwPartialError = new();
}
