using System.Web;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Sxc.Code.Internal.CodeErrorHelp;
using ToSic.Sxc.Code.Internal.SourceCode;
using ToSic.Sxc.Dnn.Razor.Internal;
using static System.StringComparer;
using static ToSic.Sxc.Configuration.Internal.SxcFeatures;
using IFeaturesService = ToSic.Sxc.Services.IFeaturesService;

namespace ToSic.Sxc.Dnn.Razor;

/// <summary>
/// Helper in Dnn to replace the HtmlHelper for the `@Html.Raw()` or `@Html.Partial()`
/// </summary>
[PrivateApi]
internal class HtmlHelper(
    LazySvc<CodeErrorHelpService> codeErrService,
    LazySvc<IFeaturesService> featureSvc,
    LazySvc<SourceAnalyzer> codeAnalysis)
    : ServiceBase("Dnn.HtmHlp", connect: [codeErrService, featureSvc, codeAnalysis]), IHtmlHelper
{
    public HtmlHelper Init(RazorComponentBase page, DnnRazorHelper helper, bool isSystemAdmin)
    {
        _page = page;
        _helper = helper;
        _isSystemAdmin = isSystemAdmin;
        return this;
    }
    private RazorComponentBase _page;
    private DnnRazorHelper _helper;
    private bool _isSystemAdmin;

    /// <inheritdoc/>
    public IHtmlString Raw(object stringHtml)
    {
        if (stringHtml == null) return new HtmlString(string.Empty);
        if (stringHtml is string s) return new HtmlString(s);
        if (stringHtml is IHtmlString h) return h;
        var ex = new ArgumentException($@"Html.Raw does not support type '{stringHtml.GetType().Name}'.", nameof(stringHtml));
        _helper.Add(ex);
        throw ex;
    }

    /// <summary>
    /// This should duplicate the way .net core does RenderPage - and should become the standard way of doing it in 2sxc
    /// </summary>
    /// <param name="path"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public IHtmlString Partial(string path, object data = default)
    {
        try
        {
            // This will get a HelperResult object, which is often not executed yet
            var result = RenderWithRoslynOrClassic(path, data);

            // In case we should throw a nice error, we must get the HTML now, to possibly cause the error and show an alternate message
            if (!ThrowPartialError)
                return result;

            var wrappedResult = new HelperResult(writer =>
            {
                try
                {
                    result.WriteTo(writer);
                }
                catch (Exception renderException)
                {
                    var nice = TryToLogAndReWrapError(renderException, path, true);
                    writer.WriteLine(nice);
                }
            });
            return wrappedResult;
        }
        catch (Exception compileException)
        {
            // Ensure our error paths exist, to only report this in the system-logs once
            _errorPaths ??= new(InvariantCultureIgnoreCase);
            var isFirstOccurrence = !_errorPaths.Contains(path);
            _errorPaths.Add(path);

            // Report if first time
            var nice = TryToLogAndReWrapError(compileException, path, isFirstOccurrence, "Special exception handling - only show message");
            var htmlError = Tag.Custom(nice);
            return htmlError;
        }
    }

    /// <summary>
    /// Determine if we should use Roslyn or the classic way of rendering and do it.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private HelperResult RenderWithRoslynOrClassic(string path, object data)
    {
        var useRoslyn = _page is ICanUseRoslynCompiler;
        var l = (this as IHasLog).Log.Fn<HelperResult>($"{nameof(useRoslyn)}: {useRoslyn}");

        // We can use Roslyn
        // Classic setup without Roslyn, use the built-in RenderPage
        if (useRoslyn)
        {
            // Try to compile with Roslyn
            // Will exit if the child has an old base class which would expect PageData["..."] properties
            // Because that would be empty https://github.com/2sic/2sxc/issues/3260
            var probablyHotBuild = DnnRazorCompiler.RenderSubPage(_page, path, data);
            if (probablyHotBuild.UsesHotBuild)
                return l.Return(probablyHotBuild.Instance, "used HotBuild");
            l.A("Tried to use Roslyn, but detected old base class so will do fallback because of PageData");
        }
        return l.Return(_page.BaseRenderPage(path, data), $"default render {(data == null ? "no" : "with")} data");
    }



    private string TryToLogAndReWrapError(Exception renderException, string path, bool reportToDnn, string additionalLog = null)
    {
        // Important to know: Once this fires, the page will stop rendering more templates
        if (reportToDnn) _page.Log?.GetContents().Ex(renderException);
        if (additionalLog != null) _page.Log?.GetContents().A(additionalLog);

        // If it's a compile issue, try to find explicit help for that
        var pathOfPage = _page.NormalizePath(path);
        var razorType =codeAnalysis.Value.TypeOfVirtualPath(pathOfPage);
        var exWithHelp = codeErrService.Value.AddHelpForCompileProblems(renderException, razorType);


        // Show a nice / ugly error depending on user permissions
        // Note that if anything breaks here, it will just use the normal error - but for what breaks in here
        // Note that if withHelp already has help, it won't be extended any more
        exWithHelp = codeErrService.Value.AddHelpIfKnownError(exWithHelp, _page);
        var nice = ((ICodeApiServiceInternal)_page._CodeApiSvc)._Block.BlockBuilder.RenderingHelper.DesignErrorMessage(
            [exWithHelp], true);
        _helper.Add(exWithHelp);
        return nice;
    }

    private HashSet<string> _errorPaths;

    private bool ThrowPartialError => _throwPartialError.Get(()
        => featureSvc.Value.IsEnabled(RazorThrowPartial.NameId) ||
           _isSystemAdmin && featureSvc.Value.IsEnabled(RenderThrowPartialSystemAdmin.NameId));
    private readonly GetOnce<bool> _throwPartialError = new();
}