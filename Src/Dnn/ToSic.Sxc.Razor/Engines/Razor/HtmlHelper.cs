using System;
using System.Web;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Engines.Razor
{
    /// <summary>
    /// helper to quickly "raw" some html.
    /// </summary>
    [PrivateApi]
    public class HtmlHelper: IHtmlHelper
    {
        private readonly RazorComponentBase _page;

        public HtmlHelper(RazorComponentBase page)
        {
            _page = page;
        }
        
        /// <inheritdoc/>
        public HtmlString Raw(object stringHtml)
        {
            if(stringHtml is string s)
                return new HtmlString(s);
            if (stringHtml is HtmlString h)
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
            => _page.BaseRenderPage(path, data);
    }
}