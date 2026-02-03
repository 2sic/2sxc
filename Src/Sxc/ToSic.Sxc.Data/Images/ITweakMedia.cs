using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.Sxc.Images;

/// <summary>
/// Tweak API for various media settings.
/// Specifically meant for images and pictures.
///
/// Some methods such as PictureClass will only have an effect if used on Picture(...) methods. 
/// </summary>
/// <remarks>
/// - Added v18.03
/// - All methods return a <see cref="ITweakMedia"/> to allow chaining.
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice("New v18.03, still WIP, especially the name could still change")]
public interface ITweakMedia
{
    #region Resize Settings

    /// <summary>
    /// Configure the Resize Settings.
    /// </summary>
    /// <param name="tweak">Tweak API to customize further settings</param>
    ITweakMedia Resize(Func<ITweakResize, ITweakResize>? tweak = default);

    /// <summary>
    /// Configure the Resize Settings.
    /// </summary>
    /// <param name="name">
    /// Name of an existing configuration, such as "Lightbox".
    /// If not specified (null) will default to "Content".
    /// </param>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="tweak">Tweak API to customize further settings</param>
    public ITweakMedia Resize(
        string? name,
        NoParamOrder npo = default,
        Func<ITweakResize, ITweakResize>? tweak = default
    );

    ITweakMedia Resize(
        IResizeSettings settings,
        NoParamOrder npo = default,
        Func<ITweakResize, ITweakResize>? tweak = default
    );
    // Note: Recipe is missing

    #endregion

    #region Lightbox

    /// <summary>
    /// Force Lightbox to be enabled (or disabled)
    /// </summary>
    /// <param name="isEnabled">Optional enabled state, defaults to true</param>
    ITweakMedia LightboxEnable(bool isEnabled = true);

    /// <summary>
    /// Group name for lightbox.
    /// All images with the same group-name will be treated as an album.
    /// </summary>
    /// <param name="group"></param>
    ITweakMedia LightboxGroup(string group);


    ITweakMedia LightboxDescription(string description);

    #endregion

    #region Img

    ITweakMedia ImgClass(string imgClass);

    ITweakMedia ImgAlt(string alt);

    ITweakMedia ImgAltFallback(string imgAltFallback);

    ITweakMedia ImgAttributes(IDictionary<string, string> attributes);

    ITweakMedia ImgAttributes(IDictionary<string, object> attributes);

    ITweakMedia ImgAttributes(object attributes);

    #endregion

    #region Picture

    ITweakMedia PictureClass(string pictureClass);

    ITweakMedia PictureAttributes(IDictionary<string, string> attributes);

    ITweakMedia PictureAttributes(IDictionary<string, object> attributes);

    ITweakMedia PictureAttributes(object attributes);

    #endregion

    #region Toolbar

    ITweakMedia Toolbar(bool enabled);

    ITweakMedia Toolbar(IToolbarBuilder toolbar);

    #endregion

    #region TweakInput - attempted in v20.09 but doesn't work - reason is we would have to pass the tweak very, very deep into the system which we stopped

    ///// <inheritdoc cref="ITweakInput{IInput}.Input(IInput, NoParamOrder)"/>
    //ITweakMedia Input(string replace, NoParamOrder npo = default);

    ///// <inheritdoc cref="ITweakInput{IInput}.Input(Func{IInput}, NoParamOrder)"/>
    //ITweakMedia Input(Func<string> func, NoParamOrder npo = default);

    ///// <inheritdoc cref="ITweakInput{IInput}.Input(Func{IInput, IInput}, NoParamOrder)"/>
    //ITweakMedia Input(Func<string, string> func, NoParamOrder npo = default);


    #endregion
}
