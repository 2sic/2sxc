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
    /// </remarks>
    [PublicApi_Stable_ForUseInYourCode]
    public interface IPageService
    {
        /// <summary>
        /// How changes should be applied to the page.
        /// Default is <see cref="T:ChangeMode.Auto"/>
        /// </summary>
        [PrivateApi("not final yet")]
        PageChangeModes ChangeMode { get; set; }

        /// <summary>
        /// Add a standard base header tag.
        /// <em>new in 3.0</em>
        /// </summary>
        /// <param name="url">the optional url for the base tag - if null, will try to default to the real url for the current page</param>
        void SetBase(string url = null);

        /// <summary>
        /// Set the Page Title.
        /// It will either try to replace the placeholder (second parameter) or otherwise prefix it to the existing title. 
        /// </summary>
        void SetTitle(string value, string placeholder = null);

        /// <summary>
        /// Set the Page Description.
        /// It will either try to replace the placeholder (second parameter) or otherwise prefix it to the existing title. 
        /// </summary>
        void SetDescription(string value, string placeholder = null);

        /// <summary>
        /// Set the Page Keywords. 
        /// It will either try to replace the placeholder (second parameter) or otherwise prefix it to the existing title. 
        /// </summary>
        void SetKeywords(string value, string placeholder = null);

        /// <summary>
        /// Set the page status code if possible (it will work in DNN, but probably not in Oqtane)
        /// </summary>
        /// <param name="statusCode">An HTTP status code like 404</param>
        /// <param name="message">Message / Description text (optional) which would be included in the header</param>
        void SetHttpStatus(int statusCode, string message = null);


        /// <summary>
        /// Add a tag to the header of the page
        /// Will simply not do anything if an error occurs, like if the page object doesn't exist
        /// </summary>
        /// <param name="tag"></param>
        void AddToHead(string tag);

        /// <summary>
        /// Add a RazorBlade Tag to the headers of the page
        /// Will simply not do anything if an error occurs, like if the page object doesn't exist
        /// </summary>
        /// <param name="tag"></param>
        void AddToHead(TagBase tag);


        /// <summary>
        /// Add a standard meta header tag.
        /// You may also want AddOpenGraph or AddJsonLd
        /// </summary>
        /// <param name="name"></param>
        /// <param name="content"></param>
        void AddMeta(string name, string content);

        /// <summary>
        /// Add an open-graph header according to http://ogp.me/
        /// </summary>
        /// <param name="property">Open Graph property name, like title or image:width. 'og:' is automatically prefixed if not included</param>
        /// <param name="content">value of this property</param>
        void AddOpenGraph(string property, string content);


        /// <summary>
        /// Add a JSON-LD header according https://developers.google.com/search/docs/guides/intro-structured-data
        /// </summary>
        /// <param name="jsonString">A prepared JSON string</param>
        void AddJsonLd(string jsonString);

        /// <summary>
        /// Add a JSON-LD header according https://developers.google.com/search/docs/guides/intro-structured-data
        /// </summary>
        /// <param name="jsonObject">A object which will be converted to JSON. We recommend using dictionaries to build the object.</param>
        void AddJsonLd(object jsonObject);

        #region Icon stuff

        /// <summary>
        /// Add an Icon header tag to the Page. 
        /// </summary>
        /// <param name="path">Path to the image/icon file</param>
        /// <param name="doNotRelyOnParameterOrder">This is a dummy parameter to force the developer to name the remaining parameters - like size: 75 etc.
        /// This allows us to add more parameters in future without worrying that existing code could break. 
        /// </param>
        /// <param name="rel">the rel-text, default is 'icon'. common terms are also 'shortcut icon' or 'apple-touch-icon'</param>
        /// <param name="size">Will be used in size='#x#' tag; only relevant if you want to provide multiple separate sizes</param>
        /// <param name="type">An optional type. If not provided, will be auto-detected from known types or remain empty</param>
        void AddIcon(
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
        /// This allows us to add more parameters in future without worrying that existing code could break.
        /// </param>
        /// <param name="favicon">path to favicon, default is '/favicon.ico' </param>
        /// <param name="rels"></param>
        /// <param name="sizes"></param>
        void AddIconSet(
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
        void Activate(params string[] keys);

        #endregion

        #region Security

        [InternalApi_DoNotUse_MayChangeWithoutNotice("Beta / WIP")]
        Attribute CspWhitelistAttribute { get; }

        #endregion
    }

}
