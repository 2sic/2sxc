using ToSic.Eav.Documentation;
using ToSic.Sxc.Images;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Services
{
    /// <summary>
    /// Service to help create responsive `img` and `picture` tags the best possible way.
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("Still WIP")]
    public interface IImageService
    {
        /// <summary>
        /// Get the format information for a specific extension.
        /// Mostly used internally, you will usually not need this. 
        /// </summary>
        /// <param name="path">Path or extension</param>
        /// <returns></returns>
        /// <remarks>Only works for the basic, known image types</remarks>
        [PrivateApi("Not sure if this is needed outside...")]
        IImageFormat GetFormat(string path);

        /// <summary>
        /// Construct custom Resize-Settings as needed, either based on existing settings or starting from scratch
        /// </summary>
        /// <param name="settings">
        ///     - A standardized Image-Settings object like Settings.Images.Content - see http://r.2sxc.org/settings
        ///     - Or a dynamic object containing settings properties (this can also be a merged custom + standard settings)
        ///     - Or a specially prepared <see cref="ToSic.Sxc.Images.IResizeSettings"/> object containing all settings. If this is provided, only `factor` will still be respected, all other settings like `width` on this command will be ignored.
        /// </param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="factor">A multiplier, usually used to create urls which resize to a part of the default content-size. Eg. 0.5. It only affects sizes from the settings.</param>
        /// <param name="width">Optional width parameter. Cannot be used if `factor` is set. Usually takes the default from the `settings`.</param>
        /// <param name="height">Optional height parameter. Can only be 0 if `factor` is set, no not specify a height. Usually takes the default from the `settings`.</param>
        /// <param name="quality">Optional quality parameter. Usually takes the default from the `settings`.</param>
        /// <param name="resizeMode">Optional resize-mode, like `crop` or `max`. Usually takes the default from the `settings`.</param>
        /// <param name="scaleMode">Optional scale-mode to allow up-scaling images like `up` or `both`. Usually takes the default from the `settings`.</param>
        /// <param name="format">Optional file format like `jpg` or `png`</param>
        /// <param name="aspectRatio">Aspect Ratio width/height, only relevant if a `factor` is supplied. Usually takes default from the `settings` or is ignored. </param>
        /// <param name="parameters">
        ///     - the parameters either as `id=47&amp;name=daniel` (Dnn also supports `/id/47/name/daniel`)
        ///     - in 2sxc 12.05+ it can also be an <see cref="ToSic.Sxc.Context.IParameters"/>
        /// </param>
        /// <param name="srcSet"></param>
        /// <returns>A settings object which has all the parameters as configured</returns>
        /// <remarks>
        /// History: Added in 2sxc 13.01
        /// </remarks>
        IResizeSettings ResizeSettings(
            object settings = null,
            string noParamOrder = Eav.Parameters.Protector,
            object factor = null,
            object width = null,
            object height = null,
            object quality = null,
            string resizeMode = null,
            string scaleMode = null,
            string format = null,
            object aspectRatio = null,
            string parameters = null,
            object srcSet = null);

        IHybridHtmlString SrcSet(
            string url,
            object settings = null,
            string noParamOrder = Eav.Parameters.Protector,
            object factor = null,
            string srcSet = null
        );

        /// <summary>
        /// Get a Responsive Picture object which you can then either just show, or use to construct a more customized output as you need it.
        /// </summary>
        /// <param name="url">The image url.</param>
        /// <param name="settings">
        /// - A standardized Image-Settings object like Settings.Images.Content - see http://r.2sxc.org/settings
        /// - Or a dynamic object containing settings properties (this can also be a merged custom + standard settings)
        /// - Or a specially prepared <see cref="ToSic.Sxc.Images.IResizeSettings"/> object containing all settings.
        /// Note: If you need to construct very custom settings, use <see cref="ResizeSettings">ResizeSettings</see> to create them
        /// </param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="factor">A multiplier, usually used to create urls which resize to a part of the default content-size. Eg. 0.5. It only affects sizes from the settings.</param>
        /// <param name="srcSet">Optional string to configure what `srcset`s to generate - see [](xref:NetCode.Images.SrcSet)</param>
        /// <param name="imgAlt">`alt` attribute on the created `img` tag for SEO etc.</param>
        /// <param name="imgClass">`class` attribute on the created `img` tag</param>
        /// <returns>A ResponsivePicture object which can be rendered directly. See [](xref:NetCode.Images.Index)</returns>
        IResponsivePicture Picture(
            string url,
            string noParamOrder = Eav.Parameters.Protector,
            object settings = null,
            object factor = null,
            string srcSet = null,
            string imgAlt = null,
            string imgClass = null
        );

        /// <summary>
        /// Get a Responsive Image object which you can then either just show, or use to construct a more customized output as you need it.
        /// </summary>
        /// <param name="url">The image url.</param>
        /// <param name="settings">
        /// - A standardized Image-Settings object like Settings.Images.Content - see http://r.2sxc.org/settings
        /// - Or a dynamic object containing settings properties (this can also be a merged custom + standard settings)
        /// - Or a specially prepared <see cref="ToSic.Sxc.Images.IResizeSettings"/> object containing all settings.
        /// Note: If you need to construct very custom settings, use <see cref="ResizeSettings">ResizeSettings</see> to create them
        /// </param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="factor">A multiplier, usually used to create urls which resize to a part of the default content-size. Eg. 0.5. It only affects sizes from the settings.</param>
        /// <param name="srcSet">Optional string to configure what `srcset`s to generate - see [](xref:NetCode.Images.SrcSet)</param>
        /// <param name="imgAlt">`alt` attribute on the created `img` tag for SEO etc.</param>
        /// <param name="imgClass">`class` attribute on the created `img` tag</param>
        /// <returns>A ResponsiveImage object which can be rendered directly. See [](xref:NetCode.Images.Index)</returns>
        IResponsiveImage Img(
            string url,
            string noParamOrder = Eav.Parameters.Protector,
            object settings = null,
            object factor = null,
            string srcSet = null,
            string imgAlt = null,
            string imgClass = null);
            
    }
}
