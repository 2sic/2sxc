using System;
using System.Web;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Dnn;

namespace ToSic.Sxc.Engines.Razor
{
    /// <summary>
    /// helper to quickly "raw" some html.
    /// </summary>
    [PrivateApi("basically shouldn't be used, because @: does the same thing")]
    public class HtmlHelper: IHtmlHelper
    {
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

    }
}