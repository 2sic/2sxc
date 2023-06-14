using System;
using System.Collections.Generic;
using System.Web;
using System.Web.WebPages;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Razor.Blade;
using ToSic.Sxc.Code.Errors;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Web;
using static System.StringComparer;
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
        public HtmlHelper(RazorComponentBase page, bool isSystemAdmin)
        {
            _page = page;
            _isSystemAdmin = isSystemAdmin;
        }
        private readonly RazorComponentBase _page;
        private readonly bool _isSystemAdmin;

        /// <inheritdoc/>
        public IHtmlString Raw(object stringHtml)
        {
            if (stringHtml == null) return new HtmlString(string.Empty);
            if (stringHtml is string s) return new HtmlString(s);
            if (stringHtml is IHtmlString h) return h;
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
                        var nice = TryToLogAndReWrapError(renderException, true);
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
                var nice = TryToLogAndReWrapError(compileException, isFirstOccurrence, "Special exception handling - only show message");
                //if (isFirstOccurrence)
                //    _page.Log?.GetContents().Ex(compileException);
                //IsError = true;
                //_page.Log?.GetContents().A("Special exception handling - only show message");

                // Show a nice / ugly error depending on user permissions
                // Note that if anything breaks here, it will just use the normal error - but for what breaks in here
                //var exRewraped = _page._DynCodeRoot.GetService<CodeErrorHelpService>().AddHelpIfKnownError(compileException, false);
                //var nice = _page._DynCodeRoot.Block.BlockBuilder.RenderingHelper.DesignErrorMessage(exRewraped, isFirstOccurrence);
                var htmlError = Tag.Custom(nice);
                return htmlError;
            }
        }

        private string TryToLogAndReWrapError(Exception renderException, bool reportToDnn, string additionalLog = null)
        {
            // Important to know: Once this fires, the page will stop rendering more templates
            IsError = true;
            if (reportToDnn) _page.Log?.GetContents().Ex(renderException);
            if (additionalLog != null) _page.Log?.GetContents().A(additionalLog);
            // Show a nice / ugly error depending on user permissions
            // Note that if anything breaks here, it will just use the normal error - but for what breaks in here
            var exRewraped = _page._DynCodeRoot.GetService<CodeErrorHelpService>().AddHelpIfKnownError(renderException, false);
            var nice = _page._DynCodeRoot.Block.BlockBuilder.RenderingHelper.DesignErrorMessage(exRewraped, true);
            return nice;
        }

        [PrivateApi] public bool IsError;
        private HashSet<string> _errorPaths;

        private bool ThrowPartialError => _throwPartialError.Get(() =>
        {
            var features = _page._DynCodeRoot.GetService<IFeaturesService>();
            return features.IsEnabled(RazorThrowPartial.NameId) ||
                   _isSystemAdmin && features.IsEnabled(RenderThrowPartialSystemAdmin.NameId);
        });
        private readonly GetOnce<bool> _throwPartialError = new GetOnce<bool>();
    }
}