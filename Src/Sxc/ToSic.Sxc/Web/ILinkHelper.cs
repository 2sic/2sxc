using ToSic.Eav.Documentation;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Web
{
    /// <summary>
    /// Helpers to create links with parameters or base-tag links (important for SPAs)
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public interface ILinkHelper: INeedsCodeRoot
    {
        /// <summary>
        /// returns a link to the current page with parameters resolved in a way that DNN wants it
        /// </summary>
        /// <param name="noParamOrder">a helper to ensure that you must use named parameters. You shouldn't give it anything, but you must use all others like parameters: "id=47&amp;name=42"</param>
        /// <param name="pageId">optional page ID (TabId) - if not supplied, will use current page</param>
        /// <param name="api">optional api url "api/name/method?id=something"</param>
        /// <param name="parameters">
        ///     - the parameters either as `id=47&amp;name=daniel` (Dnn also supports `/id/47/name/daniel`)
        ///     - in 2sxc 12.05+ it can also be an <see cref="ToSic.Sxc.Context.IParameters"/>
        /// </param>
        /// <param name="type">
        ///     Optional type changes how the link is generated. Possible values are:
        /// 
        ///     - null / not specified / empty = return link as is generated
        ///     - `"full"` return link with protocol and domain. If that was missing before, it will add current protocol/domain if possible, but not on relative `./` or `../` links
        ///     - `"//"` return link with `//domain`. If that was missing before, will add current domain if possible, but not on relative `./` or `../` links
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// History
        /// * v12 added the api parameter for liking APIs of the current app
        /// * In v12.05 the type of parameters was changed from string to object, to allow <see cref="ToSic.Sxc.Context.IParameters"/> as well
        /// </remarks>
        string To(
            string noParamOrder = Eav.Parameters.Protector,
            int? pageId = null,
            string api = null,
            object parameters = null,
            string type = null
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
        /// <param name="settings">A standardized Image-Settings object like Settings.Images.Content - see http://r.2sxc.org/settings </param>
        /// <param name="factor">A multiplier, usually used to create urls which resize to a part of the default content-size. Eg. 0.5. It only affects sizes from the settings.</param>
        /// <param name="noParamOrder">a helper to ensure that you must use named parameters. You shouldn't give it anything, but you must use all others like parameters: "id=47&amp;name=42"</param>
        /// <param name="width">Optional width parameter. Cannot be used if `factor` is set. Usually takes the default from the `settings`.</param>
        /// <param name="height">Optional height parameter. Can only be 0 if `factor` is set, no not specify a height. Usually takes the default from the `settings`.</param>
        /// <param name="quality">Optional quality parameter. Usually takes the default from the `settings`.</param>
        /// <param name="resizeMode">Optional resize-mode, like `crop` or `max`. Usually takes the default from the `settings`.</param>
        /// <param name="scaleMode">Optional scale-mode to allow up-scaling images like `up` or `both`. Usually takes the default from the `settings`.</param>
        /// <param name="format">Optional file format like `jpg` or `png`</param>
        /// <param name="aspectRatio">Aspect Ratio width/height, only relevant if a `factor` is supplied. Usually takes default from the `settings` or is ignored. </param>
        /// <param name="type">
        ///     Optional type changes how the link is generated. Possible values are:
        /// 
        ///     - null / not specified / empty = return link as is generated
        ///     - `"full"` return link with protocol and domain. If that was missing before, it will add current protocol/domain if possible, but not on relative `./` or `../` links
        ///     - `"//"` return link with `//domain`. If that was missing before, will add current domain if possible, but not on relative `./` or `../` links
        /// </param>
        /// <param name="parameters">
        ///     - the parameters either as `id=47&amp;name=daniel` (Dnn also supports `/id/47/name/daniel`)
        ///     - in 2sxc 12.05+ it can also be an <see cref="ToSic.Sxc.Context.IParameters"/>
        /// </param>
        /// <remarks>
        /// Usually a factor is applied to create a link which is possibly 50% of the content-width or similar.
        /// In these cases the height is not applied but the aspectRatio is used, which usually comes from `settings` if any were provided.
        ///
        /// History
        /// - New in 2sxc 12.03
        /// - type added ca. v12.08
        /// - srcSet added v13.01
        /// </remarks>
        /// <returns></returns>
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
            string url = null,
            object settings = null,
            object factor = null,
            string noParamOrder = Eav.Parameters.Protector,
            object width = null,
            object height = null,
            object quality = null,
            string resizeMode = null,
            string scaleMode = null,
            string format = null,
            object aspectRatio = null,
            string type = null,
            object parameters = null
            );

        /// <summary>
        /// WIP v12.04 - not final
        /// Should activate debugging for Link helpers
        /// </summary>
        /// <param name="debug"></param>
        [InternalApi_DoNotUse_MayChangeWithoutNotice("just for debugging, can change at any time but for debugging it's useful")]
        void SetDebug(bool debug);

        [PrivateApi]
        string GetCurrentRequestUrl();
    }

}
