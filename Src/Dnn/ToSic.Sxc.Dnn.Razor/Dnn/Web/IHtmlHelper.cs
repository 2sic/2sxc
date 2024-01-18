using System.Web;

namespace ToSic.Sxc.Dnn.Web;

/// <summary>
/// Helper to quickly "raw" some html.
/// **Important**: When using Oqtane, the Html object has many more features - check the .net documentation. 
/// </summary>
[PublicApi]
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
    /// Render a razor file to the page.
    /// This mimics the .net core API Html.Partial() in DNN
    /// </summary>
    /// <param name="path">path/file of razor, like "../shared/_list-item.cshtml"</param>
    /// <param name="data">TODO new v16.00</param>
    /// <returns></returns>
    IHtmlString Partial(string path, object data = default);
}