using System;
using System.Web;

namespace ToSic.SexyContent.Razor.Helpers
{
    public class HtmlHelper
    {
        /// <summary>
        /// Returns a HmltString which Razor will output as Raw Html.
        /// </summary>
        public HtmlString Raw(object stringHtml)
        {
            if(stringHtml is string)
                return new HtmlString((string)stringHtml);
            if (stringHtml is HtmlString)
                return (HtmlString)stringHtml;
            if (stringHtml == null)
                return new HtmlString(String.Empty);

            throw new ArgumentException("Html.Raw does not support type '" + stringHtml.GetType().Name + "'.", "stringHtml");
        }

    }
}