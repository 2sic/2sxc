using System;
using System.Collections.Generic;
using System.Web;
using System.Web.WebPages;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Razor.Blade;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Web;
using static ToSic.Sxc.Configuration.Features.BuiltInFeatures;
using IFeaturesService = ToSic.Sxc.Services.IFeaturesService;

namespace ToSic.Sxc.Engines.Razor
{
    /// <summary>
    /// helper to quickly "raw" some html.
    /// </summary>
    [PrivateApi]
    public class HtmlHelper: IHtmlHelper
    {
        public HtmlHelper(RazorComponentBase page, bool isSystemAdmin, IFeaturesService features)
        {
            _page = page;
            _isSystemAdmin = isSystemAdmin;
            _features = features;
        }
        private readonly RazorComponentBase _page;
        private readonly bool _isSystemAdmin;
        private readonly IFeaturesService _features;

        /// <inheritdoc/>
        public IHtmlString Raw(object stringHtml)
        {
            if(stringHtml is string s)
                return new HtmlString(s);
            if (stringHtml is IHtmlString h)
                return h;
            if (stringHtml == null)
                return new HtmlString(string.Empty);

            throw new ArgumentException("Html.Raw does not support type '" + stringHtml.GetType().Name + "'.", "stringHtml");
        }

        /// <summary>
        /// This should duplicate the way .net core does RenderPage - and should become the standard way of doing it in 2sxc
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public IHtmlString Partial(string path, params object[] data)
        {
            try
            {
                // This will get a HelperResult object, which is often not executed yet
                var result = _page.BaseRenderPage(path, data);

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
                        // Important to know: Once this fires, the page will stop rendering more templates
                        _IsError = true;
                        _page.Log?.GetContents().Ex(renderException);
                        // Show a nice / ugly error depending on user permissions
                        // Note that if anything breaks here, it will just use the normal error - but for what breaks in here
                        var nice = _page._DynCodeRoot.Block.BlockBuilder.RenderingHelper.DesignErrorMessage(renderException, true);
                        writer.WriteLine(nice);
                    }
                });
                return wrappedResult;
            }
            catch (Exception compileException)
            {
                // Ensure our error paths exist, to only report this in the system-logs once
                _errorPaths = _errorPaths ?? new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
                var isFirstOccurrence = !_errorPaths.Contains(path);
                _errorPaths.Add(path);

                // Report if first time
                if (isFirstOccurrence)
                    _page.Log?.GetContents().Ex(compileException);
                _IsError = true;
                _page.Log?.GetContents().A("Special exception handling - only show message");

                // Show a nice / ugly error depending on user permissions
                // Note that if anything breaks here, it will just use the normal error - but for what breaks in here
                var nice = _page._DynCodeRoot.Block.BlockBuilder.RenderingHelper.DesignErrorMessage(compileException, isFirstOccurrence);
                var htmlError = Tag.Custom(nice);
                return htmlError;
            }
        }

        [PrivateApi]
        public bool _IsError = false;
        private HashSet<string> _errorPaths;

        private bool ThrowPartialError => _throwPartialError.Get(() =>
            _features.IsEnabled(RazorThrowPartial.NameId) || _isSystemAdmin && _features.IsEnabled(RenderThrowPartialSystemAdmin.NameId));
        private readonly GetOnce<bool> _throwPartialError = new GetOnce<bool>();
    }
}