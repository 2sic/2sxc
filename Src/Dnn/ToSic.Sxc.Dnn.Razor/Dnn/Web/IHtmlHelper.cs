using System.Web;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Dnn.Web
{
    /// <summary>
    /// Helper to quickly "raw" some html.
    /// **Important**: When using Oqtane, the Html object has many more features - check the .net documentation. 
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
        IHtmlString Raw(object stringHtml);

        /// <summary>
        /// Experimental support for Html.Partial to mimic API in .net Core 5
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [InternalApi_DoNotUse_MayChangeWithoutNotice("WIP for v12")]
        IHtmlString Partial(string path, params object[] data);
    }
}
