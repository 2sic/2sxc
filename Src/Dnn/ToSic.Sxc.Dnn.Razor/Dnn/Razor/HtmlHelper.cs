using System.Web;
using ToSic.Razor.Blade;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Code.Sys.CodeErrorHelp;
using ToSic.Sxc.Code.Sys.SourceCode;
using ToSic.Sxc.Dnn.Razor.Sys;
using ToSic.Sxc.Render.Sys;
using ToSic.Sxc.Render.Sys.Specs;
using ToSic.Sxc.Sys.Configuration;
using ToSic.Sxc.Web.Sys.LightSpeed;
using static System.StringComparer;
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
        _isSystemAdmin = isSystemAdmin;
        return this;
    }
    private RazorComponentBase _page;
    private DnnRazorHelper _helper;
    private bool _isSystemAdmin;

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

        var renderSpecs = new RenderSpecs { Data = data };

        var cacheHelper = new RazorPartialCachingHelper(_page.ExCtx.GetAppId(), normalizedPath, renderSpecs.DataDic, _page.ExCtx, featureSvc.Value, Log);

        var cached = cacheHelper.TryGetFromCache();
        if (cached != null)
        {
            cacheHelper.ProcessListener(cached);
            return l.Return(new HtmlString(cached.Html), "Returning cached result");
        }

        try
        {
            // Prepare the specs for the partial rendering - these may get changed by the Razor at runtime
            var partialSpecs = new RenderPartialSpecsWithCaching { CacheSpecs = cacheHelper.CacheSpecsRawWithModel.Disable() }; // Start with disabled, as that's the default, and then enable it if needed

            renderSpecs = renderSpecs with { PartialSpecs = partialSpecs };

            // This will get a HelperResult object, which is often not executed yet
            var result = RenderWithRoslynOrClassic(relativePath, normalizedPath, renderSpecs);

            // In case we should throw a nice error, we must get the HTML now, to possibly cause the error and show an alternate message
            // This will also not allow partial caching
            if (!ThrowPartialError)
                return l.Return(result);

            // Experimental - try to simplify returned html string - see old code below
            try
            {
                var asString = result.ToHtmlString();

                // add to cache if needed / enabled
                cacheHelper.SaveToCache(asString, partialSpecs.CacheSpecs);

                return l.Return(new HtmlString(asString), "with partial caching");
            }
            catch (Exception renderException)
            {
                var nice = TryToLogAndReWrapError(renderException, relativePath, true);
                return l.Return(new HtmlString(nice), "error");
            }
        }
        catch (Exception compileException)
        {
            // Ensure our error paths exist, to only report this in the system-logs once
            _errorPaths ??= new(InvariantCultureIgnoreCase);
            var isFirstOccurrence = !_errorPaths.Contains(relativePath);
            _errorPaths.Add(relativePath);

            // Report if first time
            var nice = TryToLogAndReWrapError(compileException, relativePath, isFirstOccurrence, "Special exception handling - only show message");
            var htmlError = Tag.Custom(nice);
            return l.Return(htmlError);
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



    private string TryToLogAndReWrapError(Exception renderException, string path, bool reportToDnn, string additionalLog = null)
    {
        // Important to know: Once this fires, the page will stop rendering more templates
        if (reportToDnn)
            _page.Log.GetContents().Ex(renderException);
        if (additionalLog != null)
            _page.Log.GetContents().A(additionalLog);

        // If it's a compile issue, try to find explicit help for that
        var pathOfPage = _page.NormalizePath(path);
        var razorType =codeAnalysis.Value.TypeOfVirtualPath(pathOfPage);
        var exWithHelp = codeErrService.Value.AddHelpForCompileProblems(renderException, razorType);


        // Show a nice / ugly error depending on user permissions
        // Note that if anything breaks here, it will just use the normal error - but for what breaks in here
        // Note that if withHelp already has help, it won't be extended anymore
        exWithHelp = codeErrService.Value.AddHelpIfKnownError(exWithHelp, _page);
        var block = _page.ExCtx.GetState<IBlock>();
        var renderHelper = renderingHelperGenerator.New().Init(block);
        var nice = renderHelper.DesignErrorMessage([exWithHelp], true);
        _helper.Add(exWithHelp);
        return nice;
    }

    private HashSet<string> _errorPaths;

    private bool ThrowPartialError => _throwPartialError.Get(()
        => featureSvc.Value.IsEnabled(SxcFeatures.RazorThrowPartial.NameId) ||
           _isSystemAdmin && featureSvc.Value.IsEnabled(SxcFeatures.RenderThrowPartialSystemAdmin.NameId));
    private readonly GetOnce<bool> _throwPartialError = new();
}