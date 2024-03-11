using System.Collections.Specialized;

namespace ToSic.Sxc.Images;

/// <summary>
/// Settings how to resize an image for the `src` or `srcset` attributes.
///
/// It's read only, to create it, use the <see cref="ToSic.Sxc.Services.IImageService"/>
/// </summary>
/// <remarks>
/// History: Released 2sxc 13.10
/// </remarks>
[PublicApi("Still WIP")]
public interface IResizeSettings
{
    /// <summary>
    /// Name of the original settings it was based on - can be null/empty.
    /// </summary>
    /// <remarks>New v17.03</remarks>
    string BasedOn { get; }

    /// <summary>
    /// Width to resize to.
    /// If 0, width will not be changed
    /// </summary>
    int Width { get; }

    /// <summary>
    /// Height to resize to.
    /// If 0, height will not be changed
    /// </summary>
    int Height { get; }

    /// <summary>
    /// Quality factor for image formats which support quality.
    /// Usually a value between 0 and 100.
    /// If 0, quality will not be changed.
    /// </summary>
    int Quality { get; }

    /// <summary>
    /// Resize mode.
    /// If empty or "(none)" will not be used. 
    /// </summary>
    string ResizeMode { get; }

    /// <summary>
    /// Scale Mode.
    /// If empty or "(none)" will not be used. 
    /// </summary>
    string ScaleMode { get; }

    /// <summary>
    /// Target format like 'jpg' or 'png'.
    /// If empty will not be used. 
    /// </summary>
    string Format { get; }

    /// <summary>
    /// The resize factor by which the original value (width/height) is scaled
    /// </summary>
    double Factor { get; }

    /// <summary>
    /// The aspect ratio to determine the height, in case no height was specified. 
    /// </summary>
    double AspectRatio { get; }

    /// <summary>
    /// Additional url parameters in case the final link would need this.
    /// Rarely used, but can be used for resize parameters which are not standard. 
    /// </summary>
    NameValueCollection Parameters { get; }

    //[PrivateApi("WIP")] 
    //bool UseFactorMap { get; }

    //[PrivateApi("WIP")]
    //bool UseAspectRatio { get; }

    ///// <summary>
    ///// Settings which are used when img/picture tags are generated with multiple resizes
    ///// </summary>
    //[InternalApi_DoNotUse_MayChangeWithoutNotice("Still WIP")]
    //AdvancedSettings Advanced { get; }
}

internal interface IResizeSettingsInternal
{
    /// <summary>
    /// Settings which are used when img/picture tags are generated with multiple resizes
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("Still WIP - will move to public once official, and then probably an IAdvancedSettings")]
    AdvancedSettings Advanced { get; }
}