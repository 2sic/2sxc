using ToSic.Razor.Blade;
using ToSic.Sxc.Web;

// ReSharper disable UnusedMember.Global
namespace ToSic.Sxc.Services;

/// <summary>
/// Make changes to the page - usually from Razor.
/// </summary>
/// <remarks>
///
/// History
/// * Introduced in v12.02 but on another namespace which still works for compatibility
/// * Moved to ToSic.Sxc.Services in v13
/// * Added ability to use placeholder `[original]` in v13.11
/// * Most commands were updated to return an empty string in v14.02 so that they could be used as inline razor (previously `void`)
/// </remarks>
[PublicApi]
public partial interface IPageService
{
    ///// <summary>
    ///// How changes should be applied to the page.
    ///// Default is <see cref="T:ChangeMode.Auto"/>
    ///// </summary>
    //[PrivateApi("not final yet")]
    //PageChangeModes ChangeMode { get; set; }

    /// <summary>
    /// Add a standard base header tag or replace it if one is already provided.
    /// </summary>
    /// <param name="url">the optional url for the base tag - if null, will try to default to the real url for the current page</param>
    /// <returns>Empty string, so it can be used on inline razor such as `@Kit.Page.SetBase(...)`</returns>
    string SetBase(string url = null);

    /// <summary>
    /// Set the Page Title. Behavior:
    ///
    /// * By default it will _prefix_ the new title - `SetTitle('My New Title - ')` = `My New Title - Blog - 2sxc.org`
    /// * You can also use the new `[original]` token like `SetTitle('[original] - My New Title')` = `Blog - 2sxc.org - My New Title`
    /// * You can add a placeholder to the page-title and tell SetTitle what it is. `SetTitle('My New Title', '2sxc.org') = `Blog - My New Title`
    /// </summary>
    /// <returns>Empty string, so it can be used on inline razor such as `@Kit.Page.SetTitle(...)`</returns>
    string SetTitle(string value, string placeholder = null);

    /// <summary>
    /// Set the Page Description.
    /// It will either try to replace the placeholder (second parameter)
    /// or _prefix_ it to the existing description (unless `[original]` is given).
    ///
    /// See also the details with placeholder or `[original]` as explained on <see cref="SetTitle"/>
    /// </summary>
    /// <returns>Empty string, so it can be used on inline razor such as `@Kit.Page.SetDescription(...)`</returns>
    string SetDescription(string value, string placeholder = null);

    /// <summary>
    /// Set the Page Keywords. 
    /// It will either try to replace the placeholder (second parameter)
    /// or _prefix_ it to the existing keywords  (unless `[original]` is given).
    ///
    /// See also the details with placeholder or `[original]` as explained on <see cref="SetTitle"/>
    /// </summary>
    /// <returns>Empty string, so it can be used on inline razor such as `@Kit.Page.SetKeywords(...)`</returns>
    string SetKeywords(string value, string placeholder = null);

    /// <summary>
    /// Set the page status code if possible (it will work in DNN, but probably not in Oqtane)
    /// </summary>
    /// <param name="statusCode">An HTTP status code like 404</param>
    /// <param name="message">Message / Description text (optional) which would be included in the header</param>
    /// <returns>Empty string, so it can be used on inline razor such as `@Kit.Page.SetHttpStatus(...)`</returns>
    string SetHttpStatus(int statusCode, string message = null);


    /// <summary>
    /// Add a tag to the header of the page
    /// Will simply not do anything if an error occurs, like if the page object doesn't exist
    /// </summary>
    /// <param name="tag"></param>
    /// <returns>Empty string, so it can be used on inline razor such as `@Kit.Page.AddToHead(...)`</returns>
    string AddToHead(string tag);

    /// <summary>
    /// Add a RazorBlade Tag to the headers of the page
    /// Will simply not do anything if an error occurs, like if the page object doesn't exist
    /// </summary>
    /// <param name="tag"></param>
    /// <returns>Empty string, so it can be used on inline razor such as `@Kit.Page.AddToHead(...)`</returns>
    string AddToHead(IHtmlTag tag);


    /// <summary>
    /// Add a standard meta header tag.
    /// You may also want to use <see cref="AddOpenGraph"/> or <see cref="AddJsonLd(string)"/>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="content"></param>
    /// <returns>Empty string, so it can be used on inline razor such as `@Kit.Page.AddMeta(...)`</returns>
    string AddMeta(string name, string content);

    /// <summary>
    /// Add an open-graph header according to http://ogp.me/
    /// </summary>
    /// <param name="property">Open Graph property name, like title or image:width. 'og:' is automatically prefixed if not included</param>
    /// <param name="content">value of this property</param>
    /// <returns>Empty string, so it can be used on inline razor such as `@Kit.Page.AddOpenGraph(...)`</returns>
    string AddOpenGraph(string property, string content);


    /// <summary>
    /// Add a JSON-LD header according https://developers.google.com/search/docs/guides/intro-structured-data
    /// </summary>
    /// <param name="jsonString">A prepared JSON string</param>
    /// <returns>Empty string, so it can be used on inline razor such as `@Kit.Page.AddJsonLd(...)`</returns>
    string AddJsonLd(string jsonString);

    /// <summary>
    /// Add a JSON-LD header according https://developers.google.com/search/docs/guides/intro-structured-data
    /// </summary>
    /// <param name="jsonObject">A object which will be converted to JSON. We recommend using dictionaries to build the object.</param>
    /// <returns>Empty string, so it can be used on inline razor such as `@Kit.Page.AddJsonLd(...)`</returns>
    string AddJsonLd(object jsonObject);

    #region Icon stuff

    /// <summary>
    /// Add an Icon header tag to the Page. 
    /// </summary>
    /// <param name="path">Path to the image/icon file</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="rel">the rel-text, default is 'icon'. common terms are also 'shortcut icon' or 'apple-touch-icon'</param>
    /// <param name="size">Will be used in size='#x#' tag; only relevant if you want to provide multiple separate sizes</param>
    /// <param name="type">An optional type. If not provided, will be auto-detected from known types or remain empty</param>
    /// <returns>Empty string, so it can be used on inline razor such as `@Kit.Page.AddIcon(...)`</returns>
    string AddIcon(
        string path,
        NoParamOrder noParamOrder = default,
        string rel = "",
        int size = 0,
        string type = null);

    /// <summary>
    /// Add a set of icons to the page
    /// </summary>
    /// <param name="path">Path to the image/icon file</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="favicon">path to favicon, default is '/favicon.ico' </param>
    /// <param name="rels"></param>
    /// <param name="sizes"></param>
    /// <returns>Empty string, so it can be used on inline razor such as `@Kit.Page.AddIconSet(...)`</returns>
    string AddIconSet(
        string path,
        NoParamOrder noParamOrder = default,
        object favicon = null,
        IEnumerable<string> rels = null,
        IEnumerable<int> sizes = null);

    #endregion

}