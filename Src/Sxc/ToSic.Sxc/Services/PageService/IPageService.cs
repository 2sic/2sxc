using System.Collections.Generic;
using ToSic.Eav.Documentation;
using ToSic.Razor.Markup;
using ToSic.Sxc.Web;

// ReSharper disable UnusedMember.Global
namespace ToSic.Sxc.Services
{

    /// <summary>
    /// Make changes to the page - usually from Razor.
    /// </summary>
    /// <remarks>
    ///
    /// History
    /// * Introduced in v12.02 but on another namespace which still works for compatibility
    /// * Moved to ToSic.Sxc.Services in v13
    /// * Added ability to use placeholder `[original]` in v13.11
    /// </remarks>
    [PublicApi]
    public interface IPageService
    {
        /// <summary>
        /// How changes should be applied to the page.
        /// Default is <see cref="T:ChangeMode.Auto"/>
        /// </summary>
        [PrivateApi("not final yet")]
        PageChangeModes ChangeMode { get; set; }

        /// <summary>
        /// Add a standard base header tag or replace it if one is already provided.
        /// </summary>
        /// <param name="url">the optional url for the base tag - if null, will try to default to the real url for the current page</param>
        /// <remarks>
        /// History: Updated in v14.02 to always return an empty string (previously `void`) to allow direct use with `@Kit.Page.SetBase(...)`
        /// </remarks>
        string SetBase(string url = null);

        /// <summary>
        /// Set the Page Title. Behavior:
        ///
        /// * By default it will _prefix_ the new title - `SetTitle('My New Title - ')` = `My New Title - Blog - 2sxc.org`
        /// * You can also use the new `[original]` token like `SetTitle('[original] - My New Title')` = `Blog - 2sxc.org - My New Title`
        /// * You can add a placeholder to the page-title and tell SetTitle what it is. `SetTitle('My New Title', '2sxc.org') = `Blog - My New Title`
        /// </summary>
        /// <remarks>
        /// History: Updated in v14.02 to always return an empty string (previously `void`) to allow direct use with `@Kit.Page.SetTitle(...)`
        /// </remarks>
        string SetTitle(string value, string placeholder = null);

        /// <summary>
        /// Set the Page Description.
        /// It will either try to replace the placeholder (second parameter)
        /// or _prefix_ it to the existing description (unless `[original]` is given).
        ///
        /// See also the details with placeholder or `[original]` as explained on <see cref="SetTitle"/>
        /// </summary>
        /// <remarks>
        /// History: Updated in v14.02 to always return an empty string (previously `void`) to allow direct use with `@Kit.Page.SetDescription(...)`
        /// </remarks>
        string SetDescription(string value, string placeholder = null);

        /// <summary>
        /// Set the Page Keywords. 
        /// It will either try to replace the placeholder (second parameter)
        /// or _prefix_ it to the existing keywords  (unless `[original]` is given).
        ///
        /// See also the details with placeholder or `[original]` as explained on <see cref="SetTitle"/>
        /// </summary>
        /// <remarks>
        /// History: Updated in v14.02 to always return an empty string (previously `void`) to allow direct use with `@Kit.Page.SetKeywords(...)`
        /// </remarks>
        string SetKeywords(string value, string placeholder = null);

        /// <summary>
        /// Set the page status code if possible (it will work in DNN, but probably not in Oqtane)
        /// </summary>
        /// <param name="statusCode">An HTTP status code like 404</param>
        /// <param name="message">Message / Description text (optional) which would be included in the header</param>
        /// <remarks>
        /// History: Updated in v14.02 to always return an empty string (previously `void`) to allow direct use with `@Kit.Page.Activate(...)`
        /// </remarks>
        string SetHttpStatus(int statusCode, string message = null);


        /// <summary>
        /// Add a tag to the header of the page
        /// Will simply not do anything if an error occurs, like if the page object doesn't exist
        /// </summary>
        /// <param name="tag"></param>
        /// <remarks>
        /// History: Updated in v14.02 to always return an empty string (previously `void`) to allow direct use with `@Kit.Page.AddToHead(...)`
        /// </remarks>
        string AddToHead(string tag);

        /// <summary>
        /// Add a RazorBlade Tag to the headers of the page
        /// Will simply not do anything if an error occurs, like if the page object doesn't exist
        /// </summary>
        /// <param name="tag"></param>
        /// <remarks>
        /// History: Updated in v14.02 to always return an empty string (previously `void`) to allow direct use with `@Kit.Page.AddToHead(...)`
        /// </remarks>
        string AddToHead(TagBase tag);


        /// <summary>
        /// Add a standard meta header tag.
        /// You may also want to use <see cref="AddOpenGraph"/> or <see cref="AddJsonLd(string)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        /// <remarks>
        /// History: Updated in v14.02 to always return an empty string (previously `void`) to allow direct use with `@Kit.Page.AddMeta(...)`
        /// </remarks>
        string AddMeta(string name, string content);

        /// <summary>
        /// Add an open-graph header according to http://ogp.me/
        /// </summary>
        /// <param name="property">Open Graph property name, like title or image:width. 'og:' is automatically prefixed if not included</param>
        /// <param name="content">value of this property</param>
        /// <remarks>
        /// History: Updated in v14.02 to always return an empty string (previously `void`) to allow direct use with `@Kit.Page.AddOpenGraph(...)`
        /// </remarks>
        string AddOpenGraph(string property, string content);


        /// <summary>
        /// Add a JSON-LD header according https://developers.google.com/search/docs/guides/intro-structured-data
        /// </summary>
        /// <param name="jsonString">A prepared JSON string</param>
        /// <remarks>
        /// History: Updated in v14.02 to always return an empty string (previously `void`) to allow direct use with `@Kit.Page.AddJsonLd(...)`
        /// </remarks>
        string AddJsonLd(string jsonString);

        /// <summary>
        /// Add a JSON-LD header according https://developers.google.com/search/docs/guides/intro-structured-data
        /// </summary>
        /// <param name="jsonObject">A object which will be converted to JSON. We recommend using dictionaries to build the object.</param>
        /// <remarks>
        /// History: Updated in v14.02 to always return an empty string (previously `void`) to allow direct use with `@Kit.Page.AddJsonLd(...)`
        /// </remarks>
        string AddJsonLd(object jsonObject);

        #region Icon stuff

        /// <summary>
        /// Add an Icon header tag to the Page. 
        /// </summary>
        /// <param name="path">Path to the image/icon file</param>
        /// <param name="doNotRelyOnParameterOrder">This is a dummy parameter to force the developer to name the remaining parameters - like size: 75 etc.
        ///     This allows us to add more parameters in future without worrying that existing code could break. 
        /// </param>
        /// <param name="rel">the rel-text, default is 'icon'. common terms are also 'shortcut icon' or 'apple-touch-icon'</param>
        /// <param name="size">Will be used in size='#x#' tag; only relevant if you want to provide multiple separate sizes</param>
        /// <param name="type">An optional type. If not provided, will be auto-detected from known types or remain empty</param>
        /// <remarks>
        /// History: Updated in v14.02 to always return an empty string (previously `void`) to allow direct use with `@Kit.Page.AddIcon(...)`
        /// </remarks>
        string AddIcon(
            string path,
            string doNotRelyOnParameterOrder = Eav.Parameters.Protector,
            string rel = "",
            int size = 0,
            string type = null);

        /// <summary>
        /// Add a set of icons to the page
        /// </summary>
        /// <param name="path">Path to the image/icon file</param>
        /// <param name="doNotRelyOnParameterOrder">This is a dummy parameter to force the developer to name the remaining parameters - like size: 75 etc.
        ///     This allows us to add more parameters in future without worrying that existing code could break.
        /// </param>
        /// <param name="favicon">path to favicon, default is '/favicon.ico' </param>
        /// <param name="rels"></param>
        /// <param name="sizes"></param>
        /// <remarks>
        /// History: Updated in v14.02 to always return an empty string (previously `void`) to allow direct use with `@Kit.Page.AddIconSet(...)`
        /// </remarks>
        string AddIconSet(
            string path,
            string doNotRelyOnParameterOrder = Eav.Parameters.Protector,
            object favicon = null,
            IEnumerable<string> rels = null,
            IEnumerable<int> sizes = null);

        #endregion

        #region Features

        /// <summary>
        /// Activate a feature on this page.
        /// Still WIP, at the moment the only relevant key can be `turnOn`
        /// </summary>
        /// <param name="keys"></param>
        /// <remarks>
        /// History: Updated in v14.02 to always return an empty string (previously `void`) to allow direct use with `@Kit.Page.Activate(...)`
        /// </remarks>
        string Activate(params string[] keys);

        #endregion

        #region Security

        /// <summary>
        /// Add common html attributes to a `script` or `link` tag to enable optimizations
        /// and [automatically whitelist in the Content Security Policy](xref:Abyss.Security.Csp.Parts#auto-white-listing-explicit)
        /// </summary>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="optimize">Activate optimize, default is true</param>
        /// <param name="priority">Optional priority of optimization. Must be more than 100 to have an effect.</param>
        /// <param name="position">Optional position of the resource (`head`, `body`, `bottom`)</param>
        /// <param name="whitelist">Automatically add to CSP-whitelist. This uses a random key to protect against XSS.</param>
        /// <returns></returns>
        /// <remarks>
        /// History: Created in 2sxc 13.10
        /// </remarks>
        IHybridHtmlString AssetAttributes(
            string noParamOrder = Eav.Parameters.Protector,
            bool optimize = true,
            int priority = 0,
            string position = null, 
            bool whitelist = true);

        /// <summary>
        /// Add a CSP rule where you also specify the name.
        ///
        /// Example: `cspService.Add("default-src", "'self'")`
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <remarks>
        /// History: Created in 2sxc 14; will always return an empty string (previously `void`) to allow direct use with `@Kit.Page.AddCsp(...)`
        /// </remarks>
        string AddCsp(string name, params string[] values);

        #endregion
    }

}
