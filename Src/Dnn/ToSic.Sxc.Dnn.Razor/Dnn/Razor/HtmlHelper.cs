using System.Web;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Code.Sys.CodeErrorHelp;
using ToSic.Sxc.Code.Sys.SourceCode;
using ToSic.Sxc.Dnn.Razor.Sys;
using ToSic.Sxc.Render.Sys;
using ToSic.Sxc.Render.Sys.Specs;
using ToSic.Sxc.Web.Sys.LightSpeed;
using IFeaturesService = ToSic.Sxc.Services.IFeaturesService;

namespace ToSic.Sxc.Dnn.Razor;

/// <summary>
/// Helper in Dnn to replace the HtmlHelper for the `@Html.Raw()` or `@Html.Partial()`
/// </summary>
[PrivateApi]
internal class HtmlHelper(
    LazySvc<CodeErrorHelpService> codeErrService,
    LazySvc<IFeaturesService> featureSvc,
    LazySvc<SourceAnalyzer> codeAnalysis,
    Generator<IRenderingHelper> renderingHelperGenerator)
    : ServiceBase("Dnn.HtmHlp", connect: [codeErrService, featureSvc, codeAnalysis, renderingHelperGenerator]), IHtmlHelper
{
    public HtmlHelper Init(RazorComponentBase page, DnnRazorHelper helper, bool isSystemAdmin)
    {
        _page = page;
        _helper = helper;
        _errorHelper = new(page, isSystemAdmin, helper, featureSvc, codeAnalysis, codeErrService, renderingHelperGenerator);
        return this;
    }
    private RazorComponentBase _page;
    private DnnRazorHelper _helper;
    private HtmlHelperErrorHelper _errorHelper;

    /// <inheritdoc/>
    public IHtmlString Raw(object stringHtml)
    {
        switch (stringHtml)
        {
            case null: return new HtmlString("");
            case string s: return new HtmlString(s);
            case IHtmlString h: return h;
            default:
                var ex = new ArgumentException($@"Html.Raw does not support type '{stringHtml.GetType().Name}'.", nameof(stringHtml));
                _helper.Add(ex);
                throw ex;
        }
    }

    /// <summary>
    /// This should duplicate the way .net core does RenderPage - and should become the standard way of doing it in 2sxc
    /// </summary>
    /// <param name="relativePath"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public IHtmlString Partial(string relativePath, object data = default)
    {
        // Figure out the real path, and make sure it's lower case
        // so the ID in a cache remains the same no matter how it was called
        var normalizedPath = _page.NormalizePath(relativePath).ToLowerInvariant();

        var l = Log.Fn<IHtmlString>($"{nameof(relativePath)}: '{relativePath}', {nameof(normalizedPath)}: '{normalizedPath}', {nameof(data)}: {data != null}");

        // Prepare RenderSpecs with data, since it may be needed to check if caching is relevant
        // Do it like this, to avoid multiple conversions of the same data
        var renderSpecs = new RenderSpecs { Data = data };

        var cacheHelper = new RazorPartialCachingHelper(_page.ExCtx.GetAppId(), normalizedPath, renderSpecs.DataDic, _page.ExCtx, featureSvc.Value, Log);

        var cached = cacheHelper.TryGetFromCache();
        if (cached != null)
        {
            cacheHelper.PageService.ReplayCachedChanges((RenderResult)cached);
            return l.Return(new HtmlString(cached.Html), "Returning cached result");
        }

        try
        {
            // Attach any specs which the cshtml may need and possibly modify to configure caching
            renderSpecs = renderSpecs with { PartialSpecs = cacheHelper.RenderPartialSpecsForRazor };

            // This will get a HelperResult object, which is often not executed yet
            var result = RenderWithRoslynOrClassic(relativePath, normalizedPath, renderSpecs);

            // In case we should throw a nice error, we must get the HTML now, to possibly cause the error and show an alternate message
            // This will also not allow partial caching
            if (!_errorHelper.ThrowPartialError)
                return l.Return(result);

            // We want to capture the rendering of the result, so we can show nice errors and cache the result if needed.
            // We must create another render result, to delay our work.
            // Otherwise, the Razor-Engine may do some strange things and not show anything at all (instead of the error)
            var wrappedResult = new HelperResult(writer =>
            {
                try
                {
                    var asString = result.ToHtmlString();
                    writer.Write(asString); // Use Write instead of WriteLine, to not introduce any extra lines/whitespace

                    // Add to cache - should only run if no exceptions were thrown
                    cacheHelper.SaveToCacheIfEnabled(asString);
                }
                catch (Exception renderException)
                {
                    var nice = _errorHelper.TryToLogAndReWrapError(renderException, relativePath, true);
                    writer.WriteLine(nice);
                }
            });
            return wrappedResult;
        }
        catch (Exception compileException)
        {
            // Ensure our error paths exist, to only report this in the system-logs once
            //_errorPaths ??= new(InvariantCultureIgnoreCase);
            var isFirstOccurrence = !_errorHelper.ErrorPaths.Contains(relativePath);
            _errorHelper.ErrorPaths.Add(relativePath);

            // Report if first time
            var nice = _errorHelper.TryToLogAndReWrapError(compileException, relativePath, isFirstOccurrence, "Special exception handling - only show message");
            var htmlError = new HtmlString(nice);
            return l.Return(htmlError, "compile error");
        }
    }

    /// <summary>
    /// Determine if we should use Roslyn or the classic way of rendering and do it.
    /// </summary>
    /// <param name="relativePath"></param>
    /// <param name="normalizedPath"></param>
    /// <param name="renderSpecs"></param>
    /// <returns></returns>
    private HelperResult RenderWithRoslynOrClassic(string relativePath, string normalizedPath, RenderSpecs renderSpecs)
    {
        var useRoslyn = _page is ICanUseRoslynCompiler;
        var l = Log.Fn<HelperResult>($"{nameof(useRoslyn)}: {useRoslyn}");

        // We can use Roslyn
        // Classic setup without Roslyn, use the built-in RenderPage
        if (!useRoslyn)
            return l.Return(_page.BaseRenderPage(relativePath, renderSpecs), $"default render {(renderSpecs.Data == null ? "no" : "with")} data");

        // Try to compile with Roslyn
        // Will exit if the child has an old base class which would expect PageData["..."] properties
        // Because that would be empty https://github.com/2sic/2sxc/issues/3260
        var preparations = DnnRazorCompiler.PrepareForRoslyn(_page, normalizedPath, renderSpecs.Data);

        // Exit if we don't use HotBuild, because then we must revert back to classic render
        // Reason is that otherwise the PageData property - used on very old classes - would not be populated
        // Doing this from our compiler is super-hard, because it would use a lot of internal Microsoft APIs
        if (preparations.SubPage.UsesHotBuild)
        {
            var probablyHotBuild = DnnRazorCompiler.ExecuteWithRoslyn(preparations, _page, renderSpecs);
            
            //if (probablyHotBuild.UsesHotBuild)
            return l.Return(probablyHotBuild.Instance, "used HotBuild");
        }

        l.A("Tried to use Roslyn, but detected old base class so will use classic Razor Engine so PageData continues to work.");
        return l.Return(_page.BaseRenderPage(relativePath, renderSpecs), $"default render {(renderSpecs.Data == null ? "no" : "with")} data");
    }

}