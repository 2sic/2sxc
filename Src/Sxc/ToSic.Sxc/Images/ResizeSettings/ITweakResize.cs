namespace ToSic.Sxc.Images;

/// <summary>
/// WIP - ATM just the properties which are needed for the ImageService
/// </summary>
/// <remarks>
/// Introduced v18.03, still WIP
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice("New v18.03, still WIP")]
public interface ITweakResize
{
    /// <summary>
    /// Optional width parameter.
    /// Cannot be used if `factor` is set (will take precedence).
    /// Usually takes the default from the `settings`.
    /// </summary>
    public ITweakResize Width(int width);

    /// <summary>
    /// Set an explicit height.
    /// </summary>
    public ITweakResize Height(int height);

    /// <summary>
    /// A multiplier, usually used to create urls which resize to a part of the default content-size. Like 0.5.
    /// </summary>
    public ITweakResize Factor(double factor);

    /// <summary>
    /// A multiplier, as string usually used to create urls which resize to a part of the default content-size. Like 0.5.
    /// Can also be a ratio or formula, like "1/2" or "1:2" so it can accept CSS-like values.
    /// </summary>
    public ITweakResize Factor(string factor);

    /// <summary>
    /// The aspect ratio to use for resizing - for width to height.
    /// </summary>
    public ITweakResize AspectRatio(double aspectRatio);

    /// <summary>
    /// The aspect ratio to use for resizing - for width to height.
    /// Can also be a ratio or formula, like "1/2" or "1:2" so it can accept CSS-like values.
    /// </summary>
    public ITweakResize AspectRatio(string aspectRatio);

    /// <summary>
    /// Set the compression quality
    /// </summary>
    public ITweakResize Quality(double quality);

    /// <summary>
    /// Set the resize mode, like 'crop', 'max', etc.
    /// </summary>
    public ITweakResize ResizeMode(string resizeMode);

    /// <summary>
    /// Set scale-mode to allow up-scaling images like `up` or `both`.
    /// </summary>
    public ITweakResize ScaleMode(string scaleMode);

    /// <summary>
    /// Set the format of the image, like 'jpg', 'png', etc.
    /// Will only accept known formats, otherwise will ignore the value.
    /// </summary>
    public ITweakResize Format(string format);

    /// <summary>
    /// Specify custom url parameters for the image, like 'cachebreak=42'
    /// </summary>
    public ITweakResize Parameters(string parameters);
}