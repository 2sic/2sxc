using ToSic.Eav.Documentation;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;

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
        /// <param name="parameters">
        /// the parameters either as "/id/47/name/daniel" or "id=47&amp;name=daniel"
        /// in 2sxc 12.05+ it can also be an <see cref="IParameters"/>
        /// </param>
        /// <param name="api">optional api url "api/my?id=something"</param>
        /// <returns></returns>
        /// <remarks>
        /// History
        /// * In v12.05 the type of parameters was changed from string to object, to allow <see cref="IParameters"/> as well
        /// </remarks>
        string To(
            string noParamOrder = Eav.Parameters.Protector,
            int? pageId = null,
            object parameters = null,
            string api = null
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
        /// <param name="absoluteUrl">Set to true to generate absolute url</param>
        /// <param name="debug">Set to true to activate detailed logging into insights</param>
        /// <remarks>
        /// Usually a factor is applied to create a link which is possibly 50% of the content-width or similar.
        /// In these cases the height is not applied but the aspectRatio is used, which usually comes from `settings` if any were provided.
        /// New in 2sxc 12.03
        /// </remarks>
        /// <returns></returns>
        [PrivateApi]
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
            bool? absoluteUrl = null);

        /// <summary>
        /// WIP v12.04 - not final
        /// Should activate debugging for Link helpers
        /// </summary>
        /// <param name="debug"></param>
        [InternalApi_DoNotUse_MayChangeWithoutNotice("just for debugging, can change at any time but for debugging it's useful")]
        void SetDebug(bool debug);

        [PrivateApi]
        string AbsoluteUrl(string virtualPath);

        [PrivateApi]
        string GetDomainName();
    }

}
