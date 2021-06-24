using ToSic.Eav.Documentation;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Web
{
    /// <summary>
    /// Helpers to create links with parameters or base-tag links (important for SPAs)
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public interface ILinkHelper
    {
        [PrivateApi("Internal")]
        void Init(IContextOfBlock context, IApp app);
        
        /// <summary>
        /// returns a link to the current page with parameters resolved in a way that DNN wants it
        /// </summary>
        /// <param name="dontRelyOnParameterOrder">a helper to ensure that you must use named parameters. You shouldn't give it anything, but you must use all others like parameters: "id=47&amp;name=42"</param>
        /// <param name="pageId">optional page ID (TabId) - if not supplied, will use current page</param>
        /// <param name="parameters">the parameters either as "/id/47/name/daniel" or "id=47&amp;name=daniel"</param>
        /// <param name="api">optional api url "api/my?id=something"</param>
        /// <returns></returns>
        string To(
            string dontRelyOnParameterOrder = Eav.Parameters.Protector,
            int? pageId = null,
            string parameters = null,
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
        /// </summary>
        /// <param name="url">The image url. Use an empty string if you want to just get the params for re-use.</param>
        /// <param name="settings">A standardized Image-Settings object like Settings.Images.Content - see http://r.2sxc.org/settings </param>
        /// <param name="factor">A multiplier, usually used to create urls which resize to a part of the default content-size. Eg. 0.5</param>
        /// <param name="dontRelyOnParameterOrder">a helper to ensure that you must use named parameters. You shouldn't give it anything, but you must use all others like parameters: "id=47&amp;name=42"</param>
        /// <param name="width">Optional width parameter. Cannot be used if `factor` is set. Usually takes the default from the `settings`.</param>
        /// <param name="height">Optional height parameter. Can only be 0 if `factor` is set, no not specify a height. Usually takes the default from the `settings`.</param>
        /// <param name="quality">Optional quality parameter. Usually takes the default from the `settings`.</param>
        /// <param name="resizeMode">Optional resize-mode, like `crop` or `max`. Usually takes the default from the `settings`.</param>
        /// <param name="scaleMode">Optional scale-mode to allow up-scaling images like `up` or `both`. Usually takes the default from the `settings`.</param>
        /// <param name="format">Optional file format like `jpg` or `png`</param>
        /// <param name="aspectRatio">Aspect Ratio width/height, only relevant if a `factor` is supplied. Usually takes default from the `settings` or is ignored. </param>
        /// <remarks>
        /// Usually a factor is applied to create a link which is possibly 50% of the content-width or similar.
        /// In these cases the height is not applied but the aspectRatio is used, which usually comes from `settings` if any were provided. 
        /// </remarks>
        /// <returns></returns>
        [PrivateApi]
        string Image(
            string url = null,
            object settings = null,
            object factor = null,
            string dontRelyOnParameterOrder = Eav.Parameters.Protector,
            object width = null,
            object height = null,
            object quality = null,
            string resizeMode = null,
            string scaleMode = null,
            string format = null,
            //object maxWidth = null,
            //object maxHeight = null,
            object aspectRatio = null);

    }
}
