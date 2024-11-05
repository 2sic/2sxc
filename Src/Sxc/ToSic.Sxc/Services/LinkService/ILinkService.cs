using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Data;

#pragma warning disable CS0108, CS0114

namespace ToSic.Sxc.Services;

/// <summary>
/// Helpers to create links to
///
/// - Pages
/// - APIs
/// - Images
///
/// As well as create base-tag links (important for SPAs)
///
/// You will never create this yourself, as get this automatically in Razor or WebAPIs on an object called `Link`.
/// </summary>
/// <remarks>
/// History
/// 
/// - Created ca. v2 as `ToSic.Sxc.Web.ILinkHelper`
/// - Moved to this new `Services.ILinkService` in v13.05. The previous name will continue to work, but newer features will be missing on that interface. 
/// </remarks>
[PublicApi]
public interface ILinkService: INeedsCodeApiService, ICanDebug
{
    /// <summary>
    /// returns a link to the current page with parameters resolved in a way that DNN wants it
    /// </summary>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="pageId">optional page ID (TabId) - if not supplied, will use current page</param>
    /// <param name="api">optional api url "api/name/method?id=something"</param>
    /// <param name="parameters">
    ///     - the parameters either as `id=47&amp;name=daniel` (Dnn also supports `/id/47/name/daniel`)
    ///     - in 2sxc 12.05+ it can also be an <see cref="Context.IParameters"/>
    /// </param>
    /// <param name="type">
    ///     Optional type changes how the link is generated. Possible values are:
    /// 
    ///     - null / not specified / empty = return link as is generated
    ///     - `"full"` return link with protocol and domain. If that was missing before, it will add current protocol/domain if possible, but not on relative `./` or `../` links
    ///     - `"//"` return link with `//domain`. If that was missing before, will add current domain if possible, but not on relative `./` or `../` links
    /// </param>
    /// <param name="language">
    /// - If not set, `null` or empty `""` will use the specified pageId (pageIds can be language specific); api would always be the current language
    /// - If set to `"current"` will adjust pageId to use the language of the current language. API will be as before, as it was already `current`
    /// - future _(not implemented yet)_ `"primary"` would link to primary language
    /// - future _(not implemented yet)_ `"en"` or `"en-us"` would link to that specific language (page and API)
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// History
    /// * v12 added the api parameter for liking APIs of the current app
    /// * In v12.05 the type of parameters was changed from string to object, to allow <see cref="Context.IParameters"/> as well
    /// * In v13.02 introduced language with "current"
    /// </remarks>
    string To(
        NoParamOrder noParamOrder = default,
        int? pageId = null,
        string api = null,
        object parameters = null,
        string type = null,
        string language = null
    );
        
    /// <summary>
    /// A base url for the current page, for use in html-base tags
    /// </summary>
    /// <returns></returns>
    string Base();

    /// <summary>
    /// Generate an Image-Resizing link base on presets or custom parameters.  
    /// It will also ensure that the final url is safe, so it will encode umlauts, spaces etc.
    /// 
    /// Note that you can basically just use presets, or set every parameter manually.
    /// 
    /// - All params are optional.
    /// - Some combinations are not valid - like setting a factor and a width doesn't make sense and will throw an error
    /// - Most parameters if set to 0 will cause a reset so that this aspect is not in the URL
    /// </summary>
    /// <param name="url">The image url. Use an empty string if you want to just get the params for re-use.</param>
    /// <param name="settings">
    /// - A settings name such as "Content", "Lightbox" etc. (new 17.06)
    /// - A standardized Image-Settings object like Settings.Images.Content - see https://go.2sxc.org/settings
    /// - Or a dynamic object containing settings properties (this can also be a merged custom + standard settings)
    /// - Or a specially prepared <see cref="Images.IResizeSettings"/> object containing all settings.
    ///   If this is provided, only `factor` will still be respected, all other settings like `width` on this command will be ignored.
    /// </param>
    /// <param name="factor">A multiplier, usually used to create urls which resize to a part of the default content-size. Like 0.5.</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="field">WIP v13.04 - not final yet</param>
    /// <param name="width">Optional width parameter. Usually takes the default from the `settings`.</param>
    /// <param name="height">Optional height parameter. Usually takes the default from the `settings`.</param>
    /// <param name="quality">Optional quality parameter. Usually takes the default from the `settings`.</param>
    /// <param name="resizeMode">Optional resize-mode, like `crop` or `max`. Usually takes the default from the `settings`.</param>
    /// <param name="scaleMode">Optional scale-mode to allow up-scaling images like `up` or `both`. Usually takes the default from the `settings`.</param>
    /// <param name="format">Optional file format like `jpg` or `png`</param>
    /// <param name="aspectRatio">Aspect Ratio width/height, only relevant if a `width` is supplied. Can't be used together with height. Usually takes default from the `settings` or is ignored. </param>
    /// <param name="type">
    ///     Optional type changes how the link is generated. Possible values are:
    /// 
    ///     - null / not specified / empty = return link as is generated
    ///     - `"full"` return link with protocol and domain. If that was missing before, it will add current protocol/domain if possible, but not on relative `./` or `../` links
    ///     - `"//"` return link with `//domain`. If that was missing before, will add current domain if possible, but not on relative `./` or `../` links
    /// </param>
    /// <param name="parameters">
    ///     - the parameters either as `id=47&amp;name=daniel` (Dnn also supports `/id/47/name/daniel`)
    ///     - in 2sxc 12.05+ it can also be an <see cref="Context.IParameters"/>
    /// </param>
    /// <remarks>
    /// Usually a factor is applied to create a link which is possibly 50% of the content-width or similar.
    /// In these cases the height is not applied but the aspectRatio is used, which usually comes from `settings` if any were provided.
    /// 
    /// History
    /// - New in 2sxc 12.03
    /// - type added ca. v12.08
    /// - Option to use <see cref="Images.IResizeSettings"/> added in v13.03
    /// - `factor` originally didn't influence width/height if provided here, updated it v13.03 to influence that as well
    /// - `field` being added in 13.04, not ready yet
    /// </remarks>
    /// <returns></returns>
    // Test comments, probably remove soon as this was never implemented like this
    ///// <param name="part">
    ///// _This is a proposal, it's not final yet_
    ///// - null/empty/default means that the link is returned as given - as provided by the 'url' parameter. if none was provided, it's a root-absolute link like `/xyz/abc?stuff#stuff`
    ///// - `full` means full protocol, domain and everything - like https://2sxc.org/dnn-tutorials/page/subpage/?product=27&filter=42#test=42
    /////     - if a url is provided without protocol, it's assumed that it's on the current site, so the current domain/protocol are added
    /////     - when no url or params was provided would just result in the domain + link to the current page as is
    /////     - if the url seems invalid (like `hello:there` or an invalid `file:593902` reference) nothing is added
    ///// - `protocol` would just return the "http", "https" or whatever.
    /////     - if no url was provided, it will assume that the current page is to be used
    /////     - if a url was provided and it has no protocol, then the current protocol is used
    /////     - if a url was provided with protocol, it would return that
    ///// - `domain` would just return the full domain like `2sxc.org`, `www.2sxc.org` or `gettingstarted.2sxc.org`
    /////     - if no url was provided, then the domain of the current page
    /////     - if the url contains a domain, then that domain
    ///// - `hash` would just return the part after the `#` (without the `#`) - if not provided, empty string
    ///// - `query` would return the part after the `?` (without the `?`- if not provided, empty string
    /////     - if no url was provided and there are magical query params (like in DNN), these would not be returned, but not dnn-internals like tabid or language
    ///// - `suffix` would return the entire suffix starting from the `?` _including_ the `?` or `#` - if nothing is there, empty string
    ///// </param>        [PrivateApi]
    string Image(
        string url = default,
        object settings = default,
        object factor = default,
        NoParamOrder noParamOrder = default,
        IField field = default,
        object width = default,
        object height = default,
        object quality = default,
        string resizeMode = default,
        string scaleMode = default,
        string format = default,
        object aspectRatio = default,
        string type = default,
        object parameters = default
    );

}