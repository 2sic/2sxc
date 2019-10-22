using System;
using System.Web;

namespace ToSic.SexyContent.Razor.Helpers
{
    /// <summary>
    /// helper to quickly "raw" some html
    /// </summary>
    public class HtmlHelper
    {
        /// <summary>
        /// Returns a HtmlString which Razor will output as Raw Html.
        /// </summary>
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

    }
}