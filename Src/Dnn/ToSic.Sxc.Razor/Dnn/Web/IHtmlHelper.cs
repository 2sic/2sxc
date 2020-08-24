using System.Web;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Dnn.Web
{
    /// <summary>
    /// helper to quickly "raw" some html
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public interface IHtmlHelper
    {
        /// <summary>
        /// Returns a HtmlString which Razor will output as Raw Html.
        /// </summary>
        /// <returns>
        /// An HtmlString object which will be not be html-encoded when added to a page with @Html.Raw(...)
        /// </returns>
        HtmlString Raw(object stringHtml);
    }
}
