using System.Web;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Render.Output.Sys;
using ToSic.Sxc.Render.Sys;
using ToSic.Sxc.Render.Sys.Specs;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sxc.Web.Sys.LightSpeed;
using ToSic.Sys.Capabilities.Features;
using ToSic.Sys.Users;
using IFeaturesService = ToSic.Sxc.Services.IFeaturesService;

namespace ToSic.Sxc.Dnn.Razor.Sys;

/// <summary>
/// Helper in Dnn to replace the HtmlHelper for the `@Html.Raw()` or `@Html.Partial()`
/// </summary>
[PrivateApi]
internal class HtmlHelper(LazySvc<IFeaturesService> featureSvc, IModulesOutputService modulesOutputService, Generator<HtmlHelperErrorHelper, HtmlHelperContextWithPaths> errHelperGenerator)
    : ServiceWithSetup<HtmlHelperContext>("Dnn.HtmHlp", connect: [featureSvc, modulesOutputService, errHelperGenerator]), IHtmlHelper
{
    private HtmlHelperTimeKeeper TimeKeeper { get; } = new();

    /// <inheritdoc/>
    public IHtmlString Raw(object stringHtml)
        => stringHtml switch
        {
            null => new HtmlString(""),
            string s => new HtmlString(s),
            IHtmlString h => h,
            _ => throw MyOptions.RazorHelper.Add(new ArgumentException($@"Html.Raw does not support type '{stringHtml.GetType().Name}'.", nameof(stringHtml)))
        };

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
        var fullOptions = new HtmlHelperContextWithPaths(MyOptions, relativePath);

        var l = Log.Fn<IHtmlString>($"{nameof(relativePath)}: '{relativePath}', {nameof(fullOptions.CacheKey)}: '{fullOptions.CacheKey}', {nameof(data)}: {data != null}", timer: true);
        var fullTime = TimeKeeper.Start(fullOptions.Normalized);

        // Prepare RenderSpecs with data, since it may be needed to check if caching is relevant
        // Do it like this, to avoid multiple conversions of the same data
        var renderSpecs = new RenderSpecs { Data = data };

        var cacheHelper = new RazorPartialCachingHelper(MyOptions.ExCtx.GetAppId(), fullOptions.CacheKey, renderSpecs.DataDic, MyOptions.ExCtx, featureSvc.Value, Log);

        var cached = cacheHelper.TryGetFromCache();
        if (cached != null)
        {
            // Make sure the effects on the page are repeated (like headers, scripts etc.)
            cacheHelper.PageService.ReplayCachedChanges((RenderResult)cached);
            return l.Return(new HtmlString(cached.Html), "Returning cached result");
        }

        // Get the error helper to assist
        var errHelper = errHelperGenerator.New(fullOptions);

        // Try to render everything
        try
        {
            // Attach any specs which the cshtml may need and possibly modify to configure caching
            renderSpecs = renderSpecs with { PartialSpecs = cacheHelper.RenderPartialSpecsForRazor };

            // This will get a HelperResult object, which is often not executed yet
            var result = RenderWithRoslynOrClassic(fullOptions, renderSpecs);
            
            // Optionally verify that the path was perfect, to indicate if it could work on Linux (new v22)
            if (BuiltInFeatures.DevFeatures.FileNamesCrossPlatform)
                CheckFileNameCompatibleWithLinux(fullOptions);

            // In case we should throw a nice error, we must get the HTML now, to possibly cause the error and show an alternate message
            // This will also not allow partial caching
            if (!errHelper.ThrowPartialError)
                return l.Return(result);

            // We want to capture the rendering of the result, so we can show nice errors and cache the result if needed.
            // We must create another render result, to delay our work.
            // Otherwise, the Razor-Engine may do some strange things and not show anything at all (instead of the error)
            var wrappedResult = new HelperResult(writer =>
            {
                try
                {
                    fullTime.Start();
                    var asString = result.ToHtmlString();
                    writer.Write(asString); // Use Write instead of WriteLine, to not introduce any extra lines/whitespace
                    fullTime.Stop();
                    l.A($"Done rendering {fullOptions.Normalized}; Length: {asString.Length}; accumulated time for this partial: {fullTime.ElapsedMilliseconds}ms");
                    // Add to cache - should only run if no exceptions were thrown
                    cacheHelper.SaveToCacheIfEnabled(asString);
                }
                catch (Exception renderException)
                {
                    var nice = errHelper.TryToLogAndReWrapError(renderException, relativePath, true);
                    writer.WriteLine(nice);
                }
            });
            fullTime.Stop();
            return l.Return(wrappedResult, $"will add to cache: {cacheHelper.IsFullyEnabled}; accumulated time: {fullTime.ElapsedMilliseconds}ms");
        }
        catch (Exception compileException)
        {
            // Ensure our error paths exist, to only report this in the system-logs once
            //_errorPaths ??= new(InvariantCultureIgnoreCase);
            var isFirstOccurrence = !errHelper.ErrorPaths.Contains(relativePath);
            errHelper.ErrorPaths.Add(relativePath);

            // Report if first time
            var nice = errHelper.TryToLogAndReWrapError(compileException, relativePath, isFirstOccurrence, "Special exception handling - only show message");
            var htmlError = new HtmlString(nice);
            return l.Return(htmlError, "compile error");
        }
    }

    /// <summary>
    /// Determine if we should use Roslyn or the classic way of rendering and do it.
    /// </summary>
    /// <returns></returns>
    private HelperResult RenderWithRoslynOrClassic(HtmlHelperContextWithPaths fullOptions, RenderSpecs renderSpecs)
    {
        var useRoslyn = MyOptions.Page is ICanUseRoslynCompiler;
        var l = Log.Fn<HelperResult>($"{nameof(useRoslyn)}: {useRoslyn}");

        // We can use Roslyn
        // Classic setup without Roslyn, use the built-in RenderPage
        if (!useRoslyn)
            return l.Return(MyOptions.Page.BaseRenderPage(fullOptions.Relative, renderSpecs), $"default render {(renderSpecs.Data == null ? "no" : "with")} data");

        // Try to compile with Roslyn
        // Will exit if the child has an old base class which would expect PageData["..."] properties
        // Because that would be empty https://github.com/2sic/2sxc/issues/3260
        var preparations = DnnRazorCompiler.PrepareForRoslyn(MyOptions.Page, fullOptions.Normalized, renderSpecs.Data);

        // Exit if we don't use HotBuild, because then we must revert back to classic render
        // Reason is that otherwise the PageData property - used on very old classes - would not be populated
        // Doing this from our compiler is super-hard, because it would use a lot of internal Microsoft APIs
        if (preparations.SubPage.UsesHotBuild)
        {
            var probablyHotBuild = DnnRazorCompiler.ExecuteWithRoslyn(preparations, MyOptions.Page, renderSpecs);
            return l.Return(probablyHotBuild.Instance, "used HotBuild");
        }

        l.A("Tried to use Roslyn, but detected old base class so will use classic Razor Engine so PageData continues to work.");
        return l.Return(MyOptions.Page.BaseRenderPage(fullOptions.Relative, renderSpecs), $"default render {(renderSpecs.Data == null ? "no" : "with")} data");
    }

    private bool CheckFileNameCompatibleWithLinux(HtmlHelperContextWithPaths fullOptions)
    {
        var l = Log.Fn<bool>();
        try
        {
            var pathResult = PathCasingValidator.IsPathOkForLinux(fullOptions);
            if (!pathResult.IsOk)
            {
                var modId = MyOptions.ExCtx.GetCmsContext().Module.Id;
                modulesOutputService.AddHint(modId, new()
                {
                    ForUserElevation = UserElevation.ContentAdmin,
                    Message = $"Path casing issue detected: {fullOptions.Relative}; {pathResult.Message}"
                });
            }
            return l.Return(pathResult.IsOk, $"path check result: {pathResult.Message}");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnFalse($"exception: {ex.Message}");
        }

    }
}

