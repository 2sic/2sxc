using System;
using System.Collections.Generic;
using System.Web;
using System.Web.WebPages;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal.CodeErrorHelp;
using ToSic.Sxc.Code.Internal.SourceCode;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Web;
using static System.StringComparer;
using static ToSic.Sxc.Configuration.Internal.SxcFeatures;
using IFeaturesService = ToSic.Sxc.Services.IFeaturesService;

namespace ToSic.Sxc.Dnn.Razor;

/// <summary>
/// helper to quickly "raw" some html.
/// </summary>
[PrivateApi]
internal class HtmlHelper: ServiceBase, IHtmlHelper
{
    private readonly LazySvc<SourceAnalyzer> _codeAnalysis;
    private readonly LazySvc<IFeaturesService> _featureSvc;
    private readonly LazySvc<CodeErrorHelpService> _codeErrService;

    public HtmlHelper(
        LazySvc<CodeErrorHelpService> codeErrService,
        LazySvc<IFeaturesService> featureSvc,
        LazySvc<SourceAnalyzer> codeAnalysis) : base("Dnn.HtmHlp")
    {
        ConnectServices(
            _codeErrService = codeErrService,
            _featureSvc = featureSvc,
            _codeAnalysis = codeAnalysis
        );
    }

    public HtmlHelper Init(RazorComponentBase page, DnnRazorHelper helper, bool isSystemAdmin, Func<string, object, HelperResult> renderPage)
    {
        _page = page;
        _helper = helper;
        _isSystemAdmin = isSystemAdmin;
        _renderPage = renderPage;
        return this;
    }
    private RazorComponentBase _page;
    private DnnRazorHelper _helper;
    private bool _isSystemAdmin;
    private Func<string, object, HelperResult> _renderPage;

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
            var result = _renderPage(path, data);

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
            _errorPaths = _errorPaths ?? new HashSet<string>(InvariantCultureIgnoreCase);
            var isFirstOccurrence = !_errorPaths.Contains(path);
            _errorPaths.Add(path);

            // Report if first time
            var nice = TryToLogAndReWrapError(compileException, path, isFirstOccurrence, "Special exception handling - only show message");
            var htmlError = Tag.Custom(nice);
            return htmlError;
        }
    }

    private string TryToLogAndReWrapError(Exception renderException, string path, bool reportToDnn, string additionalLog = null)
    {
        // Important to know: Once this fires, the page will stop rendering more templates
        if (reportToDnn) _page.Log?.GetContents().Ex(renderException);
        if (additionalLog != null) _page.Log?.GetContents().A(additionalLog);

        // If it's a compile issue, try to find explicit help for that
        var pathOfPage = _page.NormalizePath(path);
        var razorType =_codeAnalysis.Value.TypeOfVirtualPath(pathOfPage);
        var exWithHelp = _codeErrService.Value.AddHelpForCompileProblems(renderException, razorType);


        // Show a nice / ugly error depending on user permissions
        // Note that if anything breaks here, it will just use the normal error - but for what breaks in here
        // Note that if withHelp already has help, it won't be extended any more
        exWithHelp = _codeErrService.Value.AddHelpIfKnownError(exWithHelp, _page);
        var nice = ((IDynamicCodeRootInternal)_page._DynCodeRoot)._Block.BlockBuilder.RenderingHelper.DesignErrorMessage(
            [exWithHelp], true);
        _helper.Add(exWithHelp);
        return nice;
    }

    private HashSet<string> _errorPaths;

    private bool ThrowPartialError => _throwPartialError.Get(()
        => _featureSvc.Value.IsEnabled(RazorThrowPartial.NameId) ||
           _isSystemAdmin && _featureSvc.Value.IsEnabled(RenderThrowPartialSystemAdmin.NameId));
    private readonly GetOnce<bool> _throwPartialError = new();
}